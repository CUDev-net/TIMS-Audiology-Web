using System;
using System.Windows;
using System.Windows.Input;
using MvvmDialogs;
using TIMS_X.CloudAssistant.Utils;
using TIMS_X.CloudAssistant.Views;
using TIMS_X.Core.Services;

namespace TIMS_X.CloudAssistant.ViewModels
{
	public class ChangeServerViewModel : ViewModelBase, IModalDialogViewModel
	{
		public static string DEFAULT_SERVER = "https://64.25.128.238";
		public static string DEFAULT_PORT = "443";

		private readonly ICustomerService _customerService;
		private bool _forceSave;
		private string _officeCode;
		private string _port;
		private string _serverUrl;
		private bool _useAdAuthentication;

		public ChangeServerViewModel(ICustomerService customerService)
		{
			_customerService = customerService;
		}

		public ICommand CancelCommand => new RelayCommand(
			() =>
			{
				var view = (ChangeServerView)View;
				view.DialogResult = false;
				view.Close();
			}, () => true);

		public bool ForceSave
		{
			get => _forceSave;
			set
			{
				if (_forceSave != value)
				{
					_forceSave = value;
					OnPropertyChanged();
				}
			}
		}

		public string OfficeCode
		{
			get => _officeCode;
			set
			{
				if (_officeCode != value)
				{
					_officeCode = value;
					OnPropertyChanged();
				}
			}
		}

		public string Port
		{
			get => _port;
			set
			{
				if (_port != value)
				{
					_port = value;
					OnPropertyChanged();
				}
			}
		}

		public ICommand RestoreDefaultsCommand => new RelayCommand(
			() =>
			{
				ServerUrl = DEFAULT_SERVER;
				Port = DEFAULT_PORT;
				UseADAuthentication = false;
				OfficeCode = null;
			}, () => true);


		public ICommand SaveCommand => new RelayCommand(
			() =>
			{
				var valid = Validate();
				if (valid)
				{
					var view = (ChangeServerView)View;
					ForceSave = false;
					view.DialogResult = true;
					view.Close();
				}
			}, () => true);

		public string ServerUrl
		{
			get => _serverUrl;
			set
			{
				if (_serverUrl != value)
				{
					_serverUrl = value;
					OnPropertyChanged();
				}
			}
		}

		public bool UseADAuthentication
		{
			get => _useAdAuthentication;
			set
			{
				if (_useAdAuthentication != value)
				{
					_useAdAuthentication = value;
					OnPropertyChanged();
				}
			}
		}

		public bool? DialogResult => false;

		public bool CanCloseWithoutSave()
		{
			if (ForceSave)
			{
				var dialogService = Locator.Instance.Resolve<IDialogService>();
				var window = View as Window;
				var result = dialogService.ShowMessageBox(this,
					"A valid server connection is required to continue. Close TIMS Assistant?",
					"Invalid Server Connection", MessageBoxButton.YesNo, MessageBoxImage.Information);

				if (result == MessageBoxResult.Yes) Environment.Exit(0);
				return false;
			}

			return true;
		}

		public bool Validate()
		{
			Mouse.OverrideCursor = Cursors.Wait;
			var view = (ChangeServerView)View;
			var validServer = _customerService.PingAsync(ServerUrl, Port).Result;
			var validOfficeCode = false;
			if (validServer)
				validOfficeCode = _customerService.ValidateCustomerAsync(_officeCode, _serverUrl, _port).Result;
			if (!validServer)
				view.SetError("Invalid server address/port");
			else if (!validOfficeCode) view.SetError("Invalid office code");
			Mouse.OverrideCursor = null;
			return validServer && validOfficeCode;
		}
	}
}