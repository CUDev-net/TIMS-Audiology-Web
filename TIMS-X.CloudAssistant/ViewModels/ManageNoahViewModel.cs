using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Reflection;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Input;
using MvvmDialogs;
using TIMS_X.CloudAssistant.Utils;

namespace TIMS_X.CloudAssistant.ViewModels
{
	public class ManageNoahViewModel : ViewModelBase, IModalDialogViewModel
	{
		#region Constructors

		public ManageNoahViewModel(NoahManager noahManager)
		{
			_serviceTimeout = new TimeSpan(0, 0, 30);
			_noahManager = noahManager;
			(_noahClientService, _noahServerService) = _noahManager.GetNoahServices();

			_RefreshNoahServerProcess();
			_RefreshNoahClientProcess();

			_LoadNoahServerSettings();
		}

		#endregion Constructors

		#region ManageNoahViewModel Members

		public string CurrentDbInterface
		{
			get => _currentDbInterface;
			set
			{
				if (_currentDbInterface != value)
				{
					_currentDbInterface = value;
					OnPropertyChanged();
				}
			}
		}

		public bool? DialogResult => false;

		public bool IsNoahEnabled => _noahManager.IsNoahEnabled;

		public bool IsTIMSDBInterfaceConfigured
		{
			get => _isTIMSDBInterfaceConfigured;
			set
			{
				if (value != _isTIMSDBInterfaceConfigured)
				{
					_isTIMSDBInterfaceConfigured = value;
					_ToggleDatabaseInterface(_isTIMSDBInterfaceConfigured);
					OnPropertyChanged();
				}
			}
		}

		public bool NoahClientRunning => _noahClientService?.Status == ServiceControllerStatus.Running;

		public string NoahClientStartTime
		{
			get
			{
				try
				{
					return _noahClientProcess?.StartTime.ToString() ?? "???";
				}
				catch (Win32Exception)
				{
					return "???";
				}
			}
		}

		public string NoahEnabledText => _noahManager.IsNoahEnabled ? "Noah is Enabled" : "Noah is Disabled";

		public bool NoahServerRunning => _noahServerService?.Status == ServiceControllerStatus.Running;

		public string NoahServerStartTime
		{
			get
			{
				try
				{
					return _noahServerProcess?.StartTime.ToString() ?? "???";
				}
				catch (Win32Exception)
				{
					return "???";
				}
			}
		}

		public ICommand RestartNoahServicesCommand => new RelayCommand(() =>
		{
			Mouse.OverrideCursor = Cursors.Wait;
			try
			{
				// Restart noah client
				_noahClientService.Refresh();
				if (_noahClientService.Status == ServiceControllerStatus.Running)
				{
					_noahClientService.Stop();
					_noahClientService.WaitForStatus(ServiceControllerStatus.Stopped, _serviceTimeout);
				}

				_noahClientService.Start();
				_noahClientService.WaitForStatus(ServiceControllerStatus.Running, _serviceTimeout);

				_RefreshNoahClientProcess();

				// Restart noah server
				_noahServerService.Refresh();
				if (_noahServerService.Status == ServiceControllerStatus.Running)
				{
					_noahServerService.Stop();
					_noahServerService.WaitForStatus(ServiceControllerStatus.Stopped, _serviceTimeout);
				}

				_noahServerService.Start();
				_noahServerService.WaitForStatus(ServiceControllerStatus.Running, _serviceTimeout);

				//_RefreshNoahServerProcess();
				_noahManager.InitializeNoah(1, out var isNoahServerRunning, out var isNoahClientRunning,
					out var errorMsg);

				_noahServerService.Refresh();
				if (_noahServerService.Status == ServiceControllerStatus.Running)
				{
					_noahServerService.Stop();
					_noahServerService.WaitForStatus(ServiceControllerStatus.Stopped, _serviceTimeout);
				}

				_noahServerService.Start();
				_noahServerService.WaitForStatus(ServiceControllerStatus.Running, _serviceTimeout);
				_RefreshNoahServerProcess();
			}
			catch (Exception)
			{
				MessageBox.Show("TIMS Assistant must be running as administrator to perform this task.", "Error");
			}

			OnPropertyChanged(nameof(NoahServerRunning));
			Mouse.OverrideCursor = null;
		}, () => true);

		public string TimsDatabaseDll => TIMS_DATABASE_DLL;

		public ICommand ToggleDbInterfaceCommand =>
			new RelayCommand(() => { IsTIMSDBInterfaceConfigured = !IsTIMSDBInterfaceConfigured; }, () => true);

		public Process GetProcessFromService(ServiceController service)
		{
			var mo = new ManagementObject(@"Win32_service.Name='" + service.ServiceName + "'");
			var o = mo.GetPropertyValue("ProcessId");
			var processId = (int)(uint)o;
			return Process.GetProcessById(processId);
		}

		#endregion ManageNoahViewModel Members

		#region Fields

		private bool _isTIMSDBInterfaceConfigured;
		private readonly NoahManager _noahManager;
		private Process _noahClientProcess;
		private Process _noahServerProcess;
		private readonly ServiceController _noahClientService;
		private readonly ServiceController _noahServerService;
		private string _currentDbInterface;
		private const string NOAH_DATABASE_DLL = "NoahDatabaseCoreCE.dll";
		private const string TIMS_DATABASE_DLL = "TIMS-X.NoahDataInterface.4.13.dll";
		private readonly TimeSpan _serviceTimeout;

		#endregion Fields

		#region Private Members

		/// <summary>
		///     Opens the HIMSA server settings file
		/// </summary>
		/// <returns></returns>
		private void _LoadNoahServerSettings()
		{
			var currentInterfaceFile = _noahManager.GetInstalledDbInterfacePath();
			CurrentDbInterface = currentInterfaceFile;
			if (!string.IsNullOrWhiteSpace(CurrentDbInterface))
			{
				_isTIMSDBInterfaceConfigured = Path.GetFileName(CurrentDbInterface) == TimsDatabaseDll;
				OnPropertyChanged(nameof(IsTIMSDBInterfaceConfigured));
			}
		}

		private void _RefreshNoahClientProcess()
		{
			_noahClientProcess = null;
			if (_noahClientService != null) _noahClientProcess = GetProcessFromService(_noahClientService);
			OnPropertyChanged(nameof(NoahClientStartTime));
		}

		private void _RefreshNoahServerProcess()
		{
			_noahServerProcess = null;
			if (_noahServerService != null) _noahServerProcess = GetProcessFromService(_noahServerService);
			OnPropertyChanged(nameof(NoahServerStartTime));
		}

		private void _SaveNoahServerSettings(string dbInterfacePath)
		{
			if (_noahManager.SaveDbInterfacePath(dbInterfacePath)) CurrentDbInterface = dbInterfacePath;
		}

		private void _ToggleDatabaseInterface(bool useTimsDatabase)
		{
			_SaveNoahServerSettings(useTimsDatabase
				? Path.Combine(
					Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
					TimsDatabaseDll)
				: Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
					NOAH_DATABASE_DLL));
		}

		#endregion Private Members
	}
}