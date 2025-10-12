using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BusinessSystemExternalControlsLibrary.Implementation;
using BusinessSystemExternalControlsLibrary.ViewModel;
using Himsa.Noah.BusinessAPI;
using Microsoft.Win32;
using MvvmDialogs;
using TIMS_X.CloudAssistant.Utils;
using TIMS_X.CloudAssistant.Views;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;
using TIMS_X.Core.Services;
using Action = System.Action;

namespace TIMS_X.CloudAssistant.ViewModels
{
	public class NoahPatientViewModel : ViewModelBase, IModalDialogViewModel, IDisposable
	{
		private readonly BusinessAPI _businessApi;
		private readonly IPatientService _patientService;
		private readonly RelayCommand _exportCommand;
		private PatientItem _model;
		private ModuleBarControlViewModel _moduleBarDataContext;
		private readonly Action _onCloseAction;
		private WindowState _previousState;
		private SessionBrowserControlViewModel _sessionBrowserDataContext;

		public NoahPatientViewModel(IPatientService patientService, BusinessAPI businessApi, Action onCloseAction)
		{
			_businessApi = businessApi;
			_patientService = patientService;

			// Get the Module bar configuration which is stored in configuration db in order to initialize module bar
			var moduleBarConfig = new ModuleBarConfiguration(_businessApi.CurrentUser.Name);
			_moduleBarDataContext = new ModuleBarControlViewModel(moduleBarConfig);

			_moduleBarDataContext.Init(_businessApi, true);
			OnPropertyChanged(nameof(ModuleBarDataContext));
			_sessionBrowserDataContext = new SessionBrowserControlViewModel();
			_sessionBrowserDataContext.Init(_businessApi, true, true, true);
			OnPropertyChanged(nameof(SessionBrowserDataContext));
			//_sessionBrowserDataContext.Reload();
			_businessApi.BusinessAPIEvent += _BusinessApiEventHandler;
			_onCloseAction = onCloseAction;

			_exportCommand = new RelayCommand(async () =>
			{
				var saveFileDialog = new SaveFileDialog();

				saveFileDialog.Title = "Save file as...";
				saveFileDialog.Filter = "Nhax files|*.Nhax";
				saveFileDialog.FileName = _SanitizeFilename($"{Model.FirstName}_{Model.LastName}");
				saveFileDialog.RestoreDirectory = true;
				var saveFileResult = saveFileDialog.ShowDialog();

				if (saveFileResult == true)
				{
					Mouse.OverrideCursor = Cursors.Wait;
					try
					{
						var nhaxData = await _patientService.GetPatientNHAXAsync(Model.Id);
						if (nhaxData != null)
						{
							using (var fs = File.OpenWrite(saveFileDialog.FileName))
							{
								await fs.WriteAsync(nhaxData, 0, nhaxData.Length);
							}
						}
						else
						{
							Mouse.OverrideCursor = null;
							MessageBox.Show(Application.Current.MainWindow,
								"Patient has no Noah data or there was an error retreiving it.\nFile not saved.",
								"Error", MessageBoxButton.OK, MessageBoxImage.Error);
						}

						Mouse.OverrideCursor = null;
						// Start File Explorer with file selected.
						var processArgs = "/select, \"" + saveFileDialog.FileName + "\"";
						Process.Start("explorer.exe", processArgs);
					}
					catch (Exception)
					{
						Mouse.OverrideCursor = null;
						MessageBox.Show(Application.Current.MainWindow,
							"An error occurred during the export.\nPlease try again later or contact support.", "Error",
							MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}, () => Model != null && (_sessionBrowserDataContext?.SessionList?.Any() ?? false));
		}


		public NoahPatientViewModel(Action onCloseAction)
		{
			_businessApi = null;
			_onCloseAction = onCloseAction;
		}

		public ICommand CloseCommand => new RelayCommand(() =>
		{
			Model = null;
			_onCloseAction?.Invoke();
		}, () => true);

		public ICommand ExportCommand => _exportCommand;

		/// <summary>
		///     Module Bar is always enabled.
		///     Reason: The module bar is never created when Noah is not available
		///     Therefore, if it exists, it should be enabled.
		/// </summary>
		public bool IsModuleBarEnabled => true;

		public PatientItem Model
		{
			get => _model;
			set
			{
				_model = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(NoahTestingVisibility));
				OnPropertyChanged(nameof(PatientAvatarPath));
				OnPropertyChanged(nameof(PatientName));
			}
		}

		public ModuleBarControlViewModel ModuleBarDataContext
		{
			get => _moduleBarDataContext;
			set
			{
				_moduleBarDataContext = value;
				OnPropertyChanged();
			}
		}

		public Visibility NoahTestingVisibility => Model == null ? Visibility.Collapsed : Visibility.Visible;

		public string PatientAvatarPath => Model?.Gender == Gender.Female
			? "../Assets/female_patient.png"
			: "../Assets/male_patient.png";

		public string PatientName => Model == null
			? null
			: NameUtilities.FormatPatientName(Model.FirstName, Model.Initial, Model.LastName) +
			  (Model.Inactive ? " (Inactive)" : string.Empty);

		public SessionBrowserControlViewModel SessionBrowserDataContext
		{
			get => _sessionBrowserDataContext;
			set
			{
				_sessionBrowserDataContext = value;
				OnPropertyChanged();
			}
		}

		public void Dispose()
		{
			_businessApi.BusinessAPIEvent -= _BusinessApiEventHandler;
		}

		public bool? DialogResult => false;


		private void _BusinessApiEventHandler(IBusinessAPIEventArg arg)
		{
			if (arg.EventType == BusinessAPI.NOAHEventType.ActionUpdated)
			{
				//TODO: Old code clears out speech and word rec data here
			}

			switch (arg.EventType)
			{
				case BusinessAPI.NOAHEventType.ActionRemoved:
				case BusinessAPI.NOAHEventType.ActionsAdded:
				case BusinessAPI.NOAHEventType.ActionUpdated:
				case BusinessAPI.NOAHEventType.ActionAdded:
				case BusinessAPI.NOAHEventType.ActionPurged:
				case BusinessAPI.NOAHEventType.ActionRestored:
					SessionBrowserDataContext.Reload();
					RefreshAudiogramEvent?.Invoke(this, new EventArgs());
					break;
			}
		}

		private string _SanitizeFilename(string proposedFileName)
		{
			return proposedFileName.Replace("<", "_")
				.Replace(">", "_")
				.Replace(":", "_")
				.Replace("\"", "_")
				.Replace("/", "_")
				.Replace("\\", "_")
				.Replace("|", "_")
				.Replace("?", "_")
				.Replace("*", "_");
		}

		public override void OnPrivacyModeChanged()
		{
			OnPropertyChanged(nameof(PatientName));
		}


		public void OnWindowStateChanged(WindowState windowState)
		{
			var prevState = _previousState;
			_previousState = windowState;
			// Don't update the Module bar if window is minimized or maximized
			// save the state of the window whenever its WindowState property changed
			if (windowState == WindowState.Minimized)
				return;
			if (windowState == WindowState.Normal && prevState == WindowState.Minimized)
				return;
			_moduleBarDataContext.Init(windowState);
		}

		public event EventHandler RefreshAudiogramEvent;

		public void RefreshPatient()
		{
			_businessApi.ReloadCurrentPatient();
		}

		public void ReloadSessionBrowser()
		{
			var view = (NoahPatientView)View;
			view.Dispatcher.Invoke(() =>
			{
				_sessionBrowserDataContext.Reload();
				_exportCommand.RaiseCanExecuteChanged();
			});
		}

		public bool SaveChanges()
		{
			return true;
		}
	}
}