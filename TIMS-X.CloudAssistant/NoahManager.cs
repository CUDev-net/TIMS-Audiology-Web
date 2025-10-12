using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;
using BusinessSystemExternalControlsLibrary.Controls;
using Himsa.Noah.BusinessAPI;
using Microsoft.Win32;
using TIMS_X.CloudAssistant.Models;
using TIMS_X.CloudAssistant.Utils;
using TIMS_X.CloudAssistant.ViewModels;
using TIMS_X.CloudAssistant.Views;
using TIMS_X.Core.Services;
using WPFLocalizeExtension.Engine;
using Exception = System.Exception;

namespace TIMS_X.CloudAssistant
{
	public class NoahManager
	{
		public const string TIMS_DATABASE_DLL = "TIMS-X.NoahDataInterface.dll";
		private static readonly string NOAH_CLIENT_NAME = "NoahClient";
		private static readonly string NOAH_SERVER_NAME = "NoahServer";
		private static readonly string TIMS_COMPANY_NAME = "TIMS";
		private readonly IConfigurationService _configurationService;
		private readonly BusinessAPI _businessApi;
		private readonly Stopwatch _timer;

		private NoahCallbackHandler mCallbackHandler;

		public NoahManager(IConfigurationService configurationService)
		{
			_businessApi = new BusinessAPI();
			_timer = new Stopwatch();
			_configurationService = configurationService;
		}

		public bool IsNoahEnabled { get; set; }

		public bool IsTimsDbInterfaceInstalled { get; set; }

		private string _GetNoahVersionFromRegistry()
		{
			string versionString = null;
			RegistryKey regKey = null;
			RegistryKey subKey = null;
			try
			{
				regKey = Registry.LocalMachine;
				subKey = regKey.OpenSubKey(@"Software\HIMSA\InstallationInfo\Noah\v4");
				if (subKey == null) subKey = regKey.OpenSubKey(@"Software\HIMSA\InstallationInfo\Noah\v4");


				if (subKey != null)
					// retrieve the value
					versionString = (string)subKey.GetValue("Version", string.Empty);
			}
			finally
			{
				if (subKey != null)
				{
					subKey.Close();
					subKey.Dispose();
				}

				if (regKey != null)
				{
					regKey.Close();
					regKey.Dispose();
				}
			}

			return versionString;
		}

		private static int _GetTimsProcessCount()
		{
			var processName = Process.GetCurrentProcess().ProcessName;
			return Process.GetProcesses().Count(x => x.ProcessName == processName);
		}

		private bool _InitializeBusinessApi(int userId, out string errorMessage)
		{
			errorMessage = null;

			try
			{
				mCallbackHandler = new NoahCallbackHandler();
				_businessApi.Initialize(mCallbackHandler, NoahAppType.BusinessSystem | NoahAppType.PatientManger);
				_businessApi.ModulesCanSetPatient(false);
				_businessApi.MobileAppEditPatientsEnable(false);
				_businessApi.HIPAALogEnable(false);
			}
			catch (Exception ex)
			{
				errorMessage = ex.ToString();
				return false;
			}

			try
			{
				if (_UseValidateLogin())
				{
					var method = _businessApi.GetType().GetMethod("ValidateLogin",
						new[] { typeof(string), typeof(object), typeof(int).MakeByRefType() });
					object[] parameters = { "timslogin_12", userId, null };
					method.Invoke(_businessApi, parameters);
				}
				else
				{
					_businessApi.SetCurrentUserID(userId);
				}
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
				if (ex.InnerException != null) errorMessage += Environment.NewLine + ex.InnerException.Message;

				return false;
			}


			return true;
		}


		/// <summary>
		///     Registers TIMS Cloud Assistant as the preferred business manager
		/// </summary>
		private bool _RegisterApplication(out string errorMessage)
		{
			errorMessage = null;
			try
			{
				var appName = AppDomain.CurrentDomain.FriendlyName; /* Returns TIMS-X.CloudAssistant.exe */
				var appDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				var appFullPath = Path.Combine(appDirectory ?? string.Empty, appName);
				BusinessAPI.RegisterNoahApp(
					TIMS_COMPANY_NAME,
					appName,
					NoahAppType.BusinessSystem | NoahAppType.PatientManger,
					appFullPath,
					appDirectory,
					string.Empty,
					true);
				return true;
			}
			catch (NullReferenceException)
			{
				errorMessage = "Noah cannot find the database interface. Please contact TIMS Support";
				return false;
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
				return false;
			}
		}

		private bool _UseValidateLogin()
		{
			var version = _GetNoahVersionFromRegistry();
			var segments = version.Split('.');
			if (segments.Length < 2)
				return false;
			return int.Parse(segments[1]) > 9;
		}

		public void CreateModuleBar(Grid container, NoahPatientViewModel viewModel)
		{
			container.Children.Clear();
			var moduleBar = new ModuleBarControl
			{
				Margin = new Thickness(2),
				HorizontalAlignment = HorizontalAlignment.Stretch
			};
			container.Children.Add(moduleBar);

			var moduleBarBinding = new Binding("ModuleBarDataContext")
			{
				Mode = BindingMode.OneWay,
				Source = viewModel
			};
			moduleBar.SetBinding(FrameworkElement.DataContextProperty, moduleBarBinding);

			var moduleBarEnabledBinding = new Binding("IsModuleBarEnabled")
			{
				Mode = BindingMode.OneWay,
				Source = viewModel
			};
			moduleBar.SetBinding(UIElement.IsEnabledProperty, moduleBarEnabledBinding);
		}

		public void CreateSessionBrowser(Grid container, NoahPatientViewModel viewModel)
		{
			container.Children.Clear();
			var sessionBrowser = new SessionBrowserControl
			{
				Height = 300,
				HorizontalAlignment = HorizontalAlignment.Stretch
			};
			container.Children.Add(sessionBrowser);

			var sessionBrowserBinding = new Binding("SessionBrowserDataContext")
			{
				Mode = BindingMode.OneWay,
				Source = viewModel
			};
			sessionBrowser.SetBinding(FrameworkElement.DataContextProperty, sessionBrowserBinding);
		}

		public Tuple<bool, bool> EnsureNoahServicesAreRunning()
		{
			var noahClientStatus = false;
			var noahServerStatus = false;

			var (noahClient, noahServer) = GetNoahServices();
			var timeoutDuration = TimeSpan.FromSeconds(1);
			try
			{
				if (noahClient.Status != ServiceControllerStatus.Running)
				{
					noahClient.Start();
					noahClient.WaitForStatus(ServiceControllerStatus.Running, timeoutDuration);
				}

				noahClientStatus = noahClient.Status == ServiceControllerStatus.Running;
			}
			catch (Exception)
			{
				noahClientStatus = false;
			}

			try
			{
				if (noahServer.Status != ServiceControllerStatus.Running)
				{
					noahServer.Start();
					noahServer.WaitForStatus(ServiceControllerStatus.Running, timeoutDuration);
				}

				noahServerStatus = noahServer.Status == ServiceControllerStatus.Running;
			}
			catch (Exception)
			{
				noahServerStatus = false;
			}

			return new Tuple<bool, bool>(noahClientStatus, noahServerStatus);
		}

		public BusinessAPI GetBusinessApi()
		{
			return _businessApi;
		}

		public string GetInstalledDbInterfacePath()
		{
			string result = null;
			try
			{
				var settingsFile = Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
					@"HIMSA\Noah\ServerSettings.xml");

				using (var stream = File.Open(settingsFile, FileMode.Open, FileAccess.Read, FileShare.None))
				{
					var settings = XElement.Load(stream);
					var items = settings.Descendants("item");
					foreach (var item in items)
					{
						var key =
							(from p in item.Descendants("string")
								where
									p.Value == @"NoahServer\ManagedDatabaseInterface"
								select p).FirstOrDefault();
						if (key != null)
						{
							// Found!
							var data = (from p in item.Descendants("Data")
								select p).FirstOrDefault();

							if (data != null) result = data.Value;
						}
					}
				}
			}
			catch (Exception)
			{
				// Do Nothing
			}

			return result;
		}


		public string GetLogFileContents()
		{
			string result = null;
			try
			{
				result = File.ReadAllText(_configurationService.Settings.DbInterfaceLogFile);
			}
			catch
			{
				// Do Nothing
			}

			return result;
		}

		public NoahLicense GetNoahLicense()
		{
			var license = new NoahLicense();
			if (IsNoahEnabled)
			{
				license.User = _businessApi.LicenseInfo.FirstName + " " + _businessApi.LicenseInfo.LastName;
				license.Company = _businessApi.LicenseInfo.Company;
				license.SerialNumber = _businessApi.LicenseInfo.SerialNumber;
				license.LicenseType = _businessApi.LicenseInfo.LicenseType.ToString();
				license.NoahVersion = _GetNoahVersionFromRegistry();
			}

			return license;
		}

		public Tuple<ServiceController, ServiceController> GetNoahServices()
		{
			var allServices = ServiceController.GetServices();

			var noahClientService = allServices.FirstOrDefault(x => x.ServiceName == NOAH_CLIENT_NAME);
			var noahServerService = allServices.FirstOrDefault(x => x.ServiceName == NOAH_SERVER_NAME);

			return new Tuple<ServiceController, ServiceController>(noahClientService, noahServerService);
		}

		public Tuple<bool, bool> GetNoahServicesStatus()
		{
			var noahClientStatus = false;
			var noahServerStatus = false;

			var (noahClient, noahServer) = GetNoahServices();
			noahClientStatus = noahClient.Status == ServiceControllerStatus.Running;
			noahServerStatus = noahServer.Status == ServiceControllerStatus.Running;

			return new Tuple<bool, bool>(noahClientStatus, noahServerStatus);
		}

		public string GetNoahVersionFromRegistry()
		{
			return _GetNoahVersionFromRegistry();
		}

		public bool InitializeNoah(int userId, out bool isNoahServerRunning, out bool isNoahClientRunning,
			out string errorMessage)
		{
			isNoahClientRunning = false;
			isNoahServerRunning = false;
			IsNoahEnabled = false;
			var installedDll = GetInstalledDbInterfacePath();
			IsTimsDbInterfaceInstalled = Path.GetFileName(installedDll) == TIMS_DATABASE_DLL;
			try
			{
				var processCount = _GetTimsProcessCount();
				if (processCount > 1)
				{
					errorMessage = "Only one instance of TIMS Cloud Assistant can be running at a time.";
					return false;
				}

				(isNoahClientRunning, isNoahServerRunning) = EnsureNoahServicesAreRunning();


				if (!isNoahClientRunning || !isNoahServerRunning)
				{
					errorMessage = string.Empty;
					if (!isNoahServerRunning) errorMessage += "NoahServer service is turned off." + Environment.NewLine;

					if (!isNoahClientRunning) errorMessage += "NoahClient service is turned off." + Environment.NewLine;

					errorMessage += "Run TIMS Assistant as Administrator or ensure Noah Services are running." +
					                Environment.NewLine;
					return false;
				}

				if (!_RegisterApplication(out errorMessage)) return false;

				if (!_InitializeBusinessApi(userId, out errorMessage)) return false;
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
				return false;
			}

			IsNoahEnabled = true;
			errorMessage = null;
			return true;
		}


		public bool SaveDbInterfacePath(string dbInterfacePath)
		{
			try
			{
				var settingsFile = Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
					@"HIMSA\Noah\ServerSettings.xml");
				XElement xSettings;
				using (var stream = File.Open(settingsFile, FileMode.Open, FileAccess.Read, FileShare.None))
				{
					xSettings = XElement.Load(stream);
				}

				using (var stream = new FileStream(settingsFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
				{
					var items = xSettings.Descendants("item");
					foreach (var item in items)
					{
						var key =
							(from p in item.Descendants("string")
								where
									p.Value == @"NoahServer\ManagedDatabaseInterface"
								select p).FirstOrDefault();
						if (key != null)
						{
							// Found!
							var data = (from p in item.Descendants("Data")
								select p).FirstOrDefault();

							if (data != null) data.Value = dbInterfacePath;
						}
					}

					xSettings.Save(stream);
				}


				IsTimsDbInterfaceInstalled = Path.GetFileName(dbInterfacePath) == TIMS_DATABASE_DLL;
				return true;
			}
			catch (Exception)
			{
				MessageBox.Show("TIMS Assistant must be running as administrator to perform this task.", "Error");
				return false;
			}
		}

		public async Task<bool> SwitchPatientAsync(int patientId)
		{
			var result = false;
			await Task.Run(() =>
			{
				try
				{
					result = _businessApi.SetCurrentPatientID(patientId, out var lockedByUser);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					throw;
				}
			});
			return result;
		}

		public void TrySwitchPatient(int patientId, NoahPatientViewModel viewModel, MainViewModel mainViewModel)
		{
			viewModel.IsBusy = true;
			Mouse.OverrideCursor = Cursors.Wait;
			_timer.Restart();
			SwitchPatientAsync(patientId)
				.ContinueWith(task =>
				{
					_timer.Stop();
					if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
						if (task.Result)
						{
							var ci = new CultureInfo(_businessApi.LanguageID == 0 ? 1033 : _businessApi.LanguageID);
							LocalizeDictionary.Instance.Culture = ci;
							viewModel.ReloadSessionBrowser();
						}

					((NoahPatientView)viewModel.View).Dispatcher.Invoke(() =>
					{
						viewModel.IsBusy = false;
						Mouse.OverrideCursor = null;

						mainViewModel.PatientLoadTime = _timer.Elapsed.TotalSeconds;
					});
				});
		}
	}
}