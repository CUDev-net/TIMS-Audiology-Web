using System.Reflection;
using MvvmDialogs;

namespace TIMS_X.CloudAssistant.ViewModels
{
	public class AboutViewModel : ViewModelBase, IModalDialogViewModel
	{
		private string _noahLicensee;
		private string _noahVersion;
		private string _officeCode;
		private string _serverUrl;
		private string _serverVersion;
		private string _user;
		private string _version;

		public string NoahLicensee
		{
			get => _noahLicensee;
			set
			{
				_noahLicensee = value;
				OnPropertyChanged();
			}
		}

		public string NoahVersion
		{
			get => _noahVersion;
			set
			{
				_noahVersion = value;
				OnPropertyChanged();
			}
		}

		public string OfficeCode
		{
			get => _officeCode;
			set
			{
				_officeCode = value;
				OnPropertyChanged();
			}
		}

		public string ServerUrl
		{
			get => _serverUrl;
			set
			{
				_serverUrl = value;
				OnPropertyChanged();
			}
		}

		public string ServerVersion
		{
			get => _serverVersion;
			set
			{
				_serverVersion = value;
				OnPropertyChanged();
			}
		}

		public string User
		{
			get => _user;
			set
			{
				_user = value;
				OnPropertyChanged();
			}
		}

		public string Version
		{
			get => _version;
			set
			{
				_version = value;
				OnPropertyChanged();
			}
		}

		public string VersionText
		{
			get
			{
				var version1 = Assembly.GetExecutingAssembly().GetName().Version;
				
				return "TIMS Cloud Assistant v" + version1;
			}
		}

		public bool? DialogResult => false;
	}
}