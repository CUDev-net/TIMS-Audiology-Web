using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using TIMS_X.Core.Models;
using TIMS_X.Core.Services;

namespace TIMS_X.CoreServices
{
	public class ConfigurationService : IConfigurationService
	{
		public ClientSettings Settings { get; set; }

		public void LoadSettings()
		{
			try
			{
				if (DoesSettingsFileExist())
				{
					var path = Path.Combine(
						Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
						@"TIMS\Settings.json");
					var contents = File.ReadAllText(path, Encoding.ASCII);
					Settings = string.IsNullOrWhiteSpace(contents)
						? new ClientSettings()
						: JsonConvert.DeserializeObject<ClientSettings>(contents);
				}
				else
				{
					Settings = new ClientSettings();
				}
			}
			catch (Exception)
			{
				Settings = new ClientSettings();
			}
		}

		public void SaveSettings()
		{
			try
			{
				var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
					"TIMS");
				Directory.CreateDirectory(directory);

				var path = Path.Combine(directory, "Settings.json");
				var contents = JsonConvert.SerializeObject(Settings);
				File.WriteAllText(path, contents, Encoding.ASCII);
			}
			catch (Exception)
			{
				// Do Nothing
			}
		}

		public bool DoesSettingsFileExist()
		{
			return File.Exists(Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
				@"TIMS\Settings.json"));
		}

		public string SchedulerDataServiceUrl => Settings.SchedulerApi;
		public string PatientDataServiceUrl => Settings.PatientApi;
		public string PracticeDataServiceUrl => Settings.PracticeApi;
		public string UserDataServiceUrl => Settings.UserApi;
		public string CustomerDataServiceUrl => Settings.CustomerApi;
		public string NoahDataServiceUrl => Settings.NoahApi;
		public string ProviderDataServiceUrl => Settings.ProviderApi;
	}
}