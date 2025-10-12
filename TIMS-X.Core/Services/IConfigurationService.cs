using TIMS_X.Core.Models;

namespace TIMS_X.Core.Services
{
	public interface IConfigurationService
	{
		string CustomerDataServiceUrl { get; }
		string NoahDataServiceUrl { get; }
		string PatientDataServiceUrl { get; }
		string PracticeDataServiceUrl { get; }
		string ProviderDataServiceUrl { get; }

		string SchedulerDataServiceUrl { get; }
		ClientSettings Settings { get; set; }
		string UserDataServiceUrl { get; }

		bool DoesSettingsFileExist();
		void LoadSettings();
		void SaveSettings();
	}
}