using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using MvvmDialogs;

using MvvmHelpers;

using TIMS_X.CloudAssistant.Controls;
using TIMS_X.CloudAssistant.Models;
using TIMS_X.CloudAssistant.Utils;
using TIMS_X.CloudAssistant.Views;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;
using TIMS_X.Core.Services;

namespace TIMS_X.CloudAssistant.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		#region Constructors

		public MainViewModel( IUserService userService,
			ISchedulerService schedulerService,
			IProviderService providerService,
			IPatientService patientService,
			IDialogService dialogService,
			IConfigurationService configurationService,
			IPracticeService practiceService,
			NoahManager noahManager,
			Core.ContextHelper contextHelper )
		{
			_userService = userService;
			_schedulerService = schedulerService;
			_providerService = providerService;
			_dialogService = dialogService;
			_patientService = patientService;
			_configurationService = configurationService;
			_noahManager = noahManager;
			_practiceService = practiceService;
			RecentPatientList = new ObservableRangeCollection<PatientItemEx>();
			TodayPatientList = new ObservableRangeCollection<ScheduledPatientItemEx>();
			SearchPatientList = new ObservableRangeCollection<PatientItemEx>();
			ProviderList = new ObservableRangeCollection<FilterProviderModel>();
			PatientTabs = new ObservableRangeCollection<PatientTabItem>();
			_contextHelper = contextHelper;
		}

		#endregion Constructors

		#region MainViewModel Members

		public LocalCache Cache
		{
			get; set;
		}

		public ICommand ChangeServerCommand => new RelayCommand( () =>
		{
			var view = new ChangeServerView();
			var vm = Locator.Instance.Resolve<ChangeServerViewModel>();
			vm.View = view;
			vm.ServerUrl = _configurationService.Settings.ServerUrl;
			vm.Port = _configurationService.Settings.ServerPort;
			view.DataContext = vm;
			view.Owner = (Window)View;
			var result = view.ShowDialog();

			if( result == true )
			{
				_configurationService.Settings.ServerUrl = vm.ServerUrl;
				_configurationService.Settings.ServerPort = vm.Port;
				_configurationService.SaveSettings();
			}

		}, () => true );

		public ICommand CloseTabCommand => new RelayCommand<int>(
			( int patientId ) =>
			{
				var tab = PatientTabs.FirstOrDefault( x => x.PatientId == patientId );
				if( tab != null )
				{
					var patientViewModel = (PatientViewModel)((PatientView)tab.Content).DataContext;
					if( patientViewModel.SaveChanges() )
					{
						PatientTabs.Remove( tab );
					}
				}
			},
			( int patientId ) => true );

		public ICommand ContactSupportCommand =>
			new RelayCommand( () => { Process.Start( "https://tims.screenconnect.com" ); }, () => true );

		public ICommand ExitCommand => new RelayCommand( () => { Application.Current.Shutdown(); }, () => true );

		public bool IsNoahEnabled => _noahManager.IsNoahEnabled;

		public bool IsPatientLoaded
		{
			get => _isPatientLoaded;
			set
			{
				_isPatientLoaded = value;
				OnPropertyChanged( nameof( IsPatientLoaded ) );
			}
		}

		public ICommand ManageNoahCommand => new RelayCommand( () =>
		{
			var dialog = Locator.Instance.Resolve<ManageNoahViewModel>();
			_dialogService.ShowDialog<ManageNoahView>( this, dialog );

		}, () =>
		{
			var identity = WindowsIdentity.GetCurrent();
			var principal = new WindowsPrincipal( identity );
			return principal.IsInRole( WindowsBuiltInRole.Administrator );
		} );

		public bool NoahClientStatus
		{
			get => _noahClientStatus;
			set
			{
				_noahClientStatus = value;
				OnPropertyChanged( nameof( NoahClientStatus ) );
			}
		}

		public bool NoahServerStatus
		{
			get => _noahServerStatus;
			set
			{
				_noahServerStatus = value;
				OnPropertyChanged( nameof( NoahServerStatus ) );
			}
		}

		public ICommand OnReturnPressed =>
			new RelayCommand( SearchPatients, () => !string.IsNullOrWhiteSpace( SearchText ) );

		public double PatientLoadTime
		{
			get => _patientLoadTime;
			set
			{
				_patientLoadTime = value;
				OnPropertyChanged( nameof( PatientLoadTime ) );
			}
		}

		public ObservableRangeCollection<PatientTabItem> PatientTabs
		{
			get; set;
		}

		public bool PrivacyModeEnabled
		{
			get => NameUtilities.PrivateModeEnabled;
			set
			{
				if( NameUtilities.PrivateModeEnabled != value )
				{
                    NameUtilities.PrivateModeEnabled = value;
					OnPropertyChanged( nameof( PrivacyModeEnabled ) );
                    OnPrivacyModeChanged();
                }
			}
		}

        public override void OnPrivacyModeChanged()
        {
            _noahPatientViewModel.OnPrivacyModeChanged();
            foreach (var patient in RecentPatientList)
            {
                patient.Refresh();
            }
            foreach (var patient in SearchPatientList)
            {
                patient.Refresh();
            }
            foreach (var patient in TodayPatientList)
            {
                patient.Refresh();
            }
        }

        public ObservableRangeCollection<FilterProviderModel> ProviderList
		{
			get; set;
		}

		public ObservableRangeCollection<PatientItemEx> RecentPatientList
		{
			get; set;
		}

        

		public ICommand RefreshCommand => new RelayCommand( RefreshCaches, () => true );

		public bool Refreshing
		{
			get => _refreshing;
			set
			{
				_refreshing = value;
				OnPropertyChanged( nameof( Refreshing ) );
			}
		}

		public bool Searching
		{
			get => _searching;
			set
			{
				_searching = value;
				OnPropertyChanged( nameof( Searching ) );
			}
		}

		public ObservableRangeCollection<PatientItemEx> SearchPatientList
		{
			get; set;
		}

		public ICommand SearchPatientsCommand =>
			new RelayCommand( SearchPatients, () => !string.IsNullOrWhiteSpace( SearchText ) );

		public string SearchResultsCount
		{
			get => _searchResultsCount;
			set
			{
				_searchResultsCount = value;
				OnPropertyChanged( nameof( SearchResultsCount ) );
			}
		}

		public string SearchText
		{
			get => _searchText;
			set
			{
				_searchText = value;
				OnPropertyChanged( nameof( SearchText ) );
				((RelayCommand)SearchPatientsCommand).RaiseCanExecuteChanged();
			}
		}

		public TabItem SelectedMainTab
		{
			get => _selectedMainTab;
			set
			{
				_selectedMainTab = value;
				OnPropertyChanged( nameof( SelectedMainTab ) );
			}
		}

		public PatientTabItem SelectedPatientTab
		{
			get => _selectedPatientTab;
			set
			{
				_selectedPatientTab = value;
				OnPropertyChanged( nameof( SelectedPatientTab ) );
			}
		}

		public string SelectedProviders
		{
			get
			{
				var sb = new StringBuilder();
				foreach( var provider in ProviderList.Where( x => x.IsSelected ) )
				{
					sb.Append( provider.Provider.LastFirstMiddle );
					if( provider != ProviderList.Last() )
					{
						sb.Append( ";" );
					}
				}
				return sb.ToString();
			}
		}

		public PatientItemEx SelectedRecentPatient
		{
			get => _selectedRecentPatient;
			set
			{
				_selectedRecentPatient = value;
				OnPropertyChanged( nameof( SelectedRecentPatient ) );
			}
		}

		public PatientItemEx SelectedSearchPatient
		{
			get => _selectedSearchPatient;
			set
			{
				_selectedSearchPatient = value;
				OnPropertyChanged( nameof( SelectedSearchPatient ) );
			}
		}

		public SearchType SelectedSearchType
		{
			get; set;
		}

		public ScheduledPatientItemEx SelectedTodayPatient
		{
			get => _selectedTodayPatient;
			set
			{
				_selectedTodayPatient = value;
				OnPropertyChanged( nameof( SelectedTodayPatient ) );
			}
		}

		public ICommand SelectProvidersCommand => new RelayCommand( async () =>
		{
			var viewModel = new FilterProvidersViewModel( ProviderList );
			var filterView = new FilterProvidersView();
			filterView.Owner = (Window)View;
			filterView.DataContext = viewModel;
			viewModel.View = filterView;
			filterView.ShowDialog();

			if( viewModel.DialogResult == true )
			{
				ProviderList = viewModel.ProviderList;

				var selectedProviders = ProviderList.Where( x => x.IsSelected ).Select( x => x.Provider.Id ).ToList();
				_configurationService.Settings.SelectedProviders = selectedProviders;
				_configurationService.SaveSettings();

				await _RefreshAsync();

				OnPropertyChanged( nameof( ProviderList ) );
				OnPropertyChanged( nameof( SelectedProviders ) );
				RefreshRecentPatientList();
				RefreshTodayPatientList();
			}

		}, () => true );

		public ICommand ShowAboutDialog => new RelayCommand( () =>
		{
			var noahLicense = _noahManager.GetNoahLicense();
			var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			_configurationService.LoadSettings();
			var serverVersion = _practiceService.GetServerVersionAsync().Result;
			var dialog = new AboutViewModel
			{
				User = _contextHelper.UseActiveDirectoryAuthentication && !_contextHelper.CurrentUser.IsSupport
					? _contextHelper.CurrentUser.AdDomain + "\\" + _contextHelper.CurrentUser.AdUsername
					: _contextHelper.CurrentUser.Name,
				OfficeCode = _contextHelper.OfficeCode,
				ServerUrl = _configurationService.Settings.BaseUrl,
				Version = version,
				ServerVersion = serverVersion,
				NoahVersion = noahLicense.NoahVersion,
				NoahLicensee = noahLicense.User
			};
			_dialogService.ShowDialog<AboutView>( this, dialog );

		}, () => true );

		public ObservableRangeCollection<ScheduledPatientItemEx> TodayPatientList
		{
			get; set;
		}

		public ICommand ViewLogCommand => new RelayCommand( () =>
		{
			var dialog = new LogViewModel
			{
				LogContents = _noahManager.GetLogFileContents()
			};
			_dialogService.ShowDialog<LogView>( this, dialog );

		}, () => true );

		public bool WhenSearchingIncludeInactivePatients
		{
			get => _whenSearchingIncludeInactivePatients;
			set
			{
				_whenSearchingIncludeInactivePatients = value;
				OnPropertyChanged( nameof( WhenSearchingIncludeInactivePatients ) );
			}
		}

		public async Task _RefreshAsync()
		{
			var cache = new LocalCache();
			var providers = await _providerService.GetProvidersAsync( false );
			cache.Providers = providers.OrderBy( x => x.LastFirstMiddle ).ToList();

			cache.RecentPatients = await _userService.GetRecentPatientsListAsync( _contextHelper.CurrentUser.Id );
			List<int> providerIds = new List<int>();
			if( _configurationService.Settings.SelectedProviders != null && _configurationService.Settings.SelectedProviders.Any() )
			{
				providerIds = _configurationService.Settings.SelectedProviders;
			}
			cache.TodayPatients = await _schedulerService.GetPatientsScheduledTodayAsync( providerIds );
			Cache = cache;
		}

		public void AddRecentPatient( PatientItem patient )
		{
			var existing = Cache.RecentPatients.FirstOrDefault( x => x.Id == patient.Id );
			if( existing != null )
			{
				Cache.RecentPatients.Remove( existing );
			}
			Cache.RecentPatients.Insert( 0, patient );
			if( Cache.RecentPatients.Count > 20 )
			{
				Cache.RecentPatients = Cache.RecentPatients.GetRange( 0, 20 );
			}
			RefreshRecentPatientList();
		}

		public void HandleSelectedRecentPatientDoubleClicked()
		{
			if( SelectedRecentPatient == null )
				return;
			_userService.PutRecentPatientAsync( _contextHelper.CurrentUser.Id, SelectedRecentPatient.Model.Id );
			AddRecentPatient( SelectedRecentPatient.Model );
			var view = (MainWindow)View;
			SwitchNoahPatient( SelectedRecentPatient.Model );
			var mainWindow = (MainWindow)View;
			mainWindow?.TogglePatientFlyout( false );
		}

		public void HandleSelectedSearchPatientDoubleClicked()
		{
			if( SelectedSearchPatient == null )
				return;
			_userService.PutRecentPatientAsync( _contextHelper.CurrentUser.Id, SelectedSearchPatient.Model.Id );
			AddRecentPatient( SelectedSearchPatient.Model);
			var view = (MainWindow)View;
			SwitchNoahPatient( SelectedSearchPatient.Model);
			var mainWindow = (MainWindow)View;
			mainWindow?.TogglePatientFlyout( false );
		}

		public void HandleSelectedTodayPatientDoubleClicked()
		{
			if( SelectedTodayPatient == null )
				return;
			_userService.PutRecentPatientAsync( _contextHelper.CurrentUser.Id, SelectedTodayPatient.Model.Id );
			AddRecentPatient( SelectedTodayPatient.Model);
			var view = (MainWindow)View;
			SwitchNoahPatient( SelectedTodayPatient.Model);
			var mainWindow = (MainWindow)View;
			mainWindow?.TogglePatientFlyout( false );
		}

		public void OnPatientClosed()
		{
			IsPatientLoaded = false;
		}

		public void OnWindowClosing()
		{

		}

		public void OnWindowLoaded()
		{
			var view = (MainWindow)View;
			var userId = _contextHelper.CurrentUser.Id;
			if( _contextHelper.CurrentUser.IsSupport )
			{
				userId = _userService.GetFirstUserIdAsync().Result;
			}

			if( !_noahManager.InitializeNoah( userId, out _noahServerStatus, out _noahClientStatus, out var errorMessage ) )
			{
				DelayedDispatch( 150, ( vm ) =>
				{
					var viewModel = vm as MainViewModel;
					viewModel?.ShowMessage( $"{errorMessage}\nNoah will be disabled for the remainder of this session.",
						"Noah Initialization Failure", MessageBoxButton.OK );
				} );
			}

			OnPropertyChanged( nameof( NoahServerStatus ) );
			OnPropertyChanged( nameof( NoahClientStatus ) );
			OnPropertyChanged( nameof( IsNoahEnabled ) );

			Task.Run( _RefreshAsync ).Wait();
			var filteredProviders = Cache.Providers.Select( provider => new FilterProviderModel
			{
				Provider = provider,
				IsSelected = false
			} ).ToList();

			ProviderList.ReplaceRange( filteredProviders );

			if( _configurationService.Settings.SelectedProviders != null && _configurationService.Settings.SelectedProviders.Any() )
			{
				foreach( var provider in ProviderList )
				{
					if( _configurationService.Settings.SelectedProviders.Contains( provider.Provider.Id ) )
					{
						provider.IsSelected = true;
					}
				}
			}
			else
			{
				var selectedProvider = ProviderList.FirstOrDefault( x => x.Provider.UserId == _contextHelper.CurrentUser.Id ) ??
									ProviderList.FirstOrDefault();
				if( selectedProvider != null )
				{
					selectedProvider.IsSelected = true;
				}
			}

			OnPropertyChanged( nameof( SelectedProviders ) );
			RefreshRecentPatientList();
			RefreshTodayPatientList();

			var noahPatientView = new NoahPatientView();

			if( IsNoahEnabled )
			{
				_noahPatientViewModel = new NoahPatientViewModel( _patientService, _noahManager.GetBusinessApi(), OnPatientClosed );
				_noahManager.CreateModuleBar( noahPatientView.ModuleContainer, _noahPatientViewModel );
				_noahManager.CreateSessionBrowser( noahPatientView.SessionContainer, _noahPatientViewModel );
			}
			else
			{
				_noahPatientViewModel = new NoahPatientViewModel( OnPatientClosed );
			}
			noahPatientView.DataContext = _noahPatientViewModel;
			_noahPatientViewModel.View = noahPatientView;
			view.NoahTestingPanel.Children.Add( noahPatientView );

		}

		public void OnWindowStateChanged( WindowState newWindowState )
		{
			_noahPatientViewModel.OnWindowStateChanged( newWindowState );
		}

		public void RefreshCaches()
		{
			Refreshing = true;
			Task.Run( _RefreshAsync ).ContinueWith( task =>
			{
				if( task.IsCompleted && !task.IsFaulted && !task.IsCanceled )
				{
					var view = (MainWindow)View;
					var filteredProviders = Cache.Providers.Select( provider => new FilterProviderModel
					{
						Provider = provider,
						IsSelected = false
					} ).ToList();

					// copy IsSelected values
					foreach( var provider in filteredProviders )
					{
						var existingProvider = ProviderList.FirstOrDefault( x => x.Provider.Id == provider.Provider.Id );
						if( existingProvider != null )
						{
							provider.IsSelected = existingProvider.IsSelected;
						}
					}

					view?.Dispatcher?.Invoke( () =>
					{
						ProviderList.ReplaceRange( filteredProviders );
						OnPropertyChanged( nameof( SelectedProviders ) );
						RefreshRecentPatientList();
						RefreshTodayPatientList();
						Refreshing = false;
					} );
				}
			} );
		}

		public void RefreshRecentPatientList()
		{
            if (Cache?.RecentPatients != null)
            {
                var selectedItem = _selectedRecentPatient;
                RecentPatientList.ReplaceRange(Cache.RecentPatients.Select(x => new PatientItemEx { Model = x }));
                SelectedRecentPatient = selectedItem;
            }
		}

		public void RefreshTodayPatientList()
		{
			TodayPatientList.Clear();
			if( Cache?.TodayPatients != null )
				foreach( var providerId in ProviderList.Where( x => x.IsSelected ).Select( x => x.Provider.Id ).ToList() )
					if( Cache.TodayPatients.ContainsKey( providerId ) )
						foreach( var patient in Cache.TodayPatients[providerId] )
							if( TodayPatientList.All( x => x.Model.Id != patient.Id ) )
								TodayPatientList.Add( new ScheduledPatientItemEx { Model = patient } );
		}

		public void SearchPatients()
		{
			Searching = true;
			_patientService.SearchPatientsAsync( SearchText, SelectedSearchType, WhenSearchingIncludeInactivePatients ).ContinueWith( task =>
			{
				if( task.IsCompleted && !task.IsFaulted && !task.IsCanceled )
				{
					var view = (MainWindow)View;
					var searchResults = task.Result.OrderBy( x => x.Name ).ToList();
					view.Dispatcher.Invoke( () =>
					{
						var s = searchResults.Count == 1 ? string.Empty : "s";
						SearchResultsCount = $"{searchResults.Count} Result{s}";
						SearchPatientList.ReplaceRange( searchResults.Select(x => new PatientItemEx { Model = x }) );
					} );
				}

				Searching = false;
			} );
		}

		public void SwitchNoahPatient( PatientItem selectedPatient )
		{
			if( selectedPatient == null )
				return;
			_noahManager.TrySwitchPatient( selectedPatient.Id, _noahPatientViewModel, this );
			_noahPatientViewModel.Model = selectedPatient;
			IsPatientLoaded = true;
		}

		public void SwitchPatientTab( PatientItem selectedPatient )
		{
			if( selectedPatient != null )
			{
				var existingTab = PatientTabs.FirstOrDefault( x => x.PatientId == selectedPatient.Id );
				if( existingTab == null )
				{
					var patientView = new PatientView();
					var patientViewModel = new PatientViewModel();
					patientViewModel.Model = selectedPatient;
					patientView.DataContext = patientViewModel;
					patientViewModel.View = patientView;
					var tabItem = new PatientTabItem
					{
						Header = selectedPatient.Name,
						PatientId = selectedPatient.Id,
						Content = patientView
					};
					PatientTabs.Add( tabItem );
					SelectedPatientTab = tabItem;
				}
				else
				{
					SelectedPatientTab = existingTab;
				}
			}
		}

		#endregion MainViewModel Members

		#region Fields

		private bool _isPatientLoaded;
		private bool _noahClientStatus;
		private bool _noahServerStatus;
		private bool _refreshing;
		private bool _searching;
		private bool _whenSearchingIncludeInactivePatients;
		private readonly Core.ContextHelper _contextHelper;
		private double _patientLoadTime;
		private readonly IConfigurationService _configurationService;
		private readonly IDialogService _dialogService;
		private readonly IPatientService _patientService;
		private readonly IPracticeService _practiceService;
		private readonly IProviderService _providerService;
		private readonly ISchedulerService _schedulerService;
		private readonly IUserService _userService;
		private readonly NoahManager _noahManager;
		private NoahPatientViewModel _noahPatientViewModel;
		private PatientItemEx _selectedRecentPatient;
		private PatientItemEx _selectedSearchPatient;
		private PatientTabItem _selectedPatientTab;
		private ScheduledPatientItemEx _selectedTodayPatient;
		private string _searchResultsCount;
		private string _searchText;
		private TabItem _selectedMainTab;

		#endregion Fields

	}
}