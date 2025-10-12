using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MvvmHelpers;
using TIMS_X.CloudAssistant.Utils;
using TIMS_X.CloudAssistant.Views;
using TIMS_X.Core;
using TIMS_X.Core.Models;
using TIMS_X.Core.Services;
using Application = System.Windows.Application;

namespace TIMS_X.CloudAssistant.ViewModels
{
	/*
      // create a "principal context" - e.g. your domain (could be machine, too)
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "cu"))
            {
                // validate the credentials
                bool isValid = pc.ValidateCredentials("dustins", "cudrTneed");
                Console.WriteLine("Is Valid? " + isValid);
            }
     */
	public class LoginViewModel : ViewModelBase
	{
		#region Constructors

		/// <summary>
		///     Title of the application, as displayed in the top bar of the window
		/// </summary>
		public LoginViewModel(IConfigurationService configurationService,
			IUserService userService,
			IPracticeService practiceService,
			ICustomerService customerService,
			ContextHelper contextHelper)
		{
			_userService = userService;
			_practiceService = practiceService;
			_customerService = customerService;
			_formHeader = DefaultHeader;
			_configurationService = configurationService;
			_contextHelper = contextHelper;

			Users = new ObservableRangeCollection<UserItem>();
			Sites = new ObservableRangeCollection<SiteItem>();
		}

		#endregion Constructors

		#region LoginViewModel Members

		public string FormHeader
		{
			get => _formHeader;
			set
			{
				_formHeader = value;
				OnPropertyChanged();
			}
		}

		public ICommand LogInCommand => new RelayCommand(_Login, _CanLoginExecute);

		public string Password
		{
			get => _password;
			set
			{
				_password = value;
				OnPropertyChanged();
				((RelayCommand)LogInCommand).RaiseCanExecuteChanged();
			}
		}

		public bool PasswordHidden => _contextHelper.UseActiveDirectoryAuthentication && !_verifyAdCredentials &&
		                              !_supportLoginRequested;

		public SiteItem SelectedSite
		{
			get => _selectedSite;
			set
			{
				_selectedSite = value;
				OnPropertyChanged();
			}
		}

		public UserItem SelectedUser
		{
			get => _selectedUser;
			set
			{
				_selectedUser = value;

				if (_selectedUser != null) SelectedSite = Sites.FirstOrDefault(x => x.Id == _selectedUser.SiteId);

				if (SelectedSite == null) SelectedSite = Sites.FirstOrDefault();

				OnPropertyChanged();
			}
		}

		public ICommand ServerSettingsCommand =>
			new RelayCommand(_HandleServerSettingsCommand, _CanServerSettingsCommandExecute);

		public ObservableRangeCollection<SiteItem> Sites { get; set; }

		public bool UserComboEnabled
		{
			get => _userComboEnabled;
			set
			{
				if (value != _userComboEnabled)
				{
					_userComboEnabled = value;
					OnPropertyChanged();
				}
			}
		}

		public ObservableRangeCollection<UserItem> Users { get; set; }

		public void ToggleSupportLogin()
		{
			if (!_contextHelper.UseActiveDirectoryAuthentication)
				return;
			_supportLoginRequested = !_supportLoginRequested;
			OnPropertyChanged(nameof(PasswordHidden));
		}

		public void WindowLoaded()
		{
			_RefreshScreen();
		}

		#endregion LoginViewModel Members

		#region Fields

		private bool _supportLoginRequested;
		private bool _userComboEnabled;

		// If Practice is set up to use AD Authentication but the current Windows User does not match 
		// anyone in the Users list, we enable this flag.
		private bool _verifyAdCredentials;
		private readonly IConfigurationService _configurationService;
		private readonly ICustomerService _customerService;
		private readonly IPracticeService _practiceService;
		private readonly IUserService _userService;
		private readonly ContextHelper _contextHelper;
		private SiteItem _selectedSite;
		private string _formHeader;
		private string _password;
		private static readonly string DefaultHeader = "Enter Office Code To Continue";
		private UserItem _selectedUser;

		#endregion Fields

		#region Private Members

		private bool _CanLoginExecute()
		{
			var canExecute = SelectedUser != null;

			if (!_contextHelper.UseActiveDirectoryAuthentication)
				canExecute = canExecute && !string.IsNullOrWhiteSpace(Password);

			return canExecute;
		}

		private bool _CanServerSettingsCommandExecute()
		{
			return true;
		}

		private void _HandleServerSettingsCommand()
		{
			var owner = View as Window;
			var view = new ChangeServerView { Owner = owner };

			var vm = Locator.Instance.Resolve<ChangeServerViewModel>();
			vm.View = view;
			vm.ServerUrl = _configurationService.Settings.ServerUrl;
			vm.Port = _configurationService.Settings.ServerPort;
			vm.UseADAuthentication = _configurationService.Settings.UseADAuthentication;
			vm.OfficeCode = _configurationService.Settings.OfficeCode;
			view.DataContext = vm;
			view.Owner = null;
			var result = view.ShowDialog();
			if (result == true)
			{
				_configurationService.Settings.ServerUrl = vm.ServerUrl;
				_configurationService.Settings.ServerPort = vm.Port;
				_configurationService.Settings.UseADAuthentication = vm.UseADAuthentication;
				_configurationService.Settings.OfficeCode = vm.OfficeCode;
				_configurationService.SaveSettings();
				_RefreshScreen();
			}
		}

		private void _Login()
		{
			var loginView = (LoginView)View;
			loginView.ClearErrorMessages();

			var isAuthenticated = false;

			if (_verifyAdCredentials)
			{
				if (string.IsNullOrWhiteSpace(_password))
				{
					loginView.SetPasswordError("Invalid password");
					return;
				}

				using (var pc = new PrincipalContext(ContextType.Domain, _selectedUser.AdDomain))
				{
					if (pc.ValidateCredentials(_selectedUser.AdUsername, _password))
						isAuthenticated = _userService.LoginAsync(_configurationService.Settings.OfficeCode,
							_selectedSite.Id, _selectedUser, null).Result;
				}
			}
			else
			{
				isAuthenticated = _userService.LoginAsync(_configurationService.Settings.OfficeCode, _selectedSite.Id,
					_selectedUser, _password).Result;
			}

			if (isAuthenticated)
			{
				var settings = _configurationService.Settings;
				settings.UserId = _selectedUser.Id;
				settings.SiteId = _selectedSite.Id;
				settings.WebToken = _contextHelper.CurrentUser.Jwt;
				_configurationService.SaveSettings();

				loginView.Close();
			}
			else
			{
				if (_configurationService.Settings.UseADAuthentication)
					loginView.SetPasswordError(
						"Error logging in: TIMS is not configured to use Active Directory Authentication.");
				else
					loginView.SetPasswordError("Invalid password");
			}
		}

		private void _RefreshScreen()
		{
			Mouse.OverrideCursor = Cursors.Wait;
			var loginView = (LoginView)View;
			loginView.ClearErrorMessages();
			var practiceName = _practiceService.GetPracticeNameAsync(_configurationService.Settings.OfficeCode).Result;
			FormHeader = "Log in to " + practiceName;

			_SetupUsers();
			_SetupSites();

			Mouse.OverrideCursor = null;
		}

		private void _SetupSites()
		{
			var sites = _practiceService.GetSitesAsync(false, _configurationService.Settings.OfficeCode).Result;
			Sites.ReplaceRange(sites);
			SelectedSite = Sites.FirstOrDefault();
			if (SelectedUser != null) SelectedSite = Sites.FirstOrDefault(x => x.Id == SelectedUser.SiteId);
			if (SelectedSite == null) SelectedSite = Sites.FirstOrDefault();
			//if( _configurationService.Settings.SiteId > 0 )
			//{
			//	var site = Sites.FirstOrDefault( x => x.Id == _configurationService.Settings.SiteId );
			//	if( site != null )
			//	{
			//		SelectedSite = site;
			//	}
			//}
		}

		private void _SetupUsers()
		{
			UserComboEnabled = true;
			var useAd = _practiceService.UsesAdAuthenticationAsync(_configurationService.Settings.OfficeCode).Result;
			_verifyAdCredentials = false;
			SelectedUser = null;
			var users = _userService.GetLoginUsersAsync(false, _configurationService.Settings.OfficeCode).Result;
			Users.ReplaceRange(users);

			if (useAd)
			{
				var validUsers = Users.Where(x => !string.IsNullOrWhiteSpace(x.AdUsername)).ToList();
				Users.ReplaceRange(validUsers);
				SelectedUser = Users.FirstOrDefault(a =>
					string.Equals(a.AdDomain, Environment.UserDomainName,
						StringComparison.InvariantCultureIgnoreCase) &&
					string.Equals(a.AdUsername, Environment.UserName, StringComparison.InvariantCultureIgnoreCase));
				if (SelectedUser == null)
					_verifyAdCredentials = true;
				else
					UserComboEnabled = false;
			}

			if (SelectedUser == null)
			{
				SelectedUser = Users.FirstOrDefault();

				if (_configurationService.Settings.UserId > 0)
				{
					var user = Users.FirstOrDefault(x => x.Id == _configurationService.Settings.UserId);
					if (user != null) SelectedUser = user;
				}
			}

			OnPropertyChanged(nameof(PasswordHidden));
		}

		/*

        private void OnShowAboutDialog()
        {
            Log.Info("Opening About dialog");
            AboutViewModel dialog = new AboutViewModel();
            var result = DialogService.ShowDialog<About>(this, dialog);
        }*/
		private void OnExitApp()
		{
			Application.Current.MainWindow.Close();
		}

		private void OnNewTest()
		{
			// TODO
		}

		private void OnSaveTest()
		{
			// TODO
		}

		#endregion Private Members
	}
}