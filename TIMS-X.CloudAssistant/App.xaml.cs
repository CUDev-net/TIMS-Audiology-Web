using System;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;
using Serilog;
using Serilog.Events;
using TIMS_X.CloudAssistant.ViewModels;
using TIMS_X.CloudAssistant.Views;
using TIMS_X.Core;
using TIMS_X.Core.Services;
using Application = System.Windows.Application;

namespace TIMS_X.CloudAssistant
{
    public partial class App : Application
    {
        private static MainWindow app;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // For catching Global uncaught exception
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionOccured);

            // Ensure the current culture passed into bindings is the OS culture.
            // By default, WPF uses en-US as the culture, regardless of the system settings.
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            

            Locator.Instance.Build();
            //Core.Application.Initialize(Locator.Instance.Resolve<ILogService>().ProcessLogEvent);

            //LogMachineDetails();

            app = new MainWindow();

            var configService = Locator.Instance.Resolve<IConfigurationService>();
            // File doesn't exist for first time setup. Ask user for the server url and port.
            configService.LoadSettings();

            Core.Application.Initialize((logMessage) =>
            {
                if (string.IsNullOrWhiteSpace(configService.Settings.TimsLogFile))
                    return;

                using (var streamWriter = File.AppendText(configService.Settings.TimsLogFile))
                {
                    streamWriter.WriteLine($"[{DateTime.Now:MM/dd/yyyy HH:mm tt}] {logMessage.RenderMessage()}");
                }
            });

            bool validConfiguration = configService.DoesSettingsFileExist();
            
            if(validConfiguration)
            {
                validConfiguration = !string.IsNullOrWhiteSpace(configService.Settings.ServerUrl) &&
                    !string.IsNullOrWhiteSpace(configService.Settings.ServerPort) &&
                    !string.IsNullOrWhiteSpace(configService.Settings.OfficeCode);

                if(validConfiguration)
                {
                    var customerService = Locator.Instance.Resolve<ICustomerService>();
                    var validServer = customerService.PingAsync(configService.Settings.ServerUrl, configService.Settings.ServerPort).Result;
                    var validOfficeCode = false;
                    if (validServer) validOfficeCode = customerService.ValidateCustomerAsync(configService.Settings.OfficeCode, 
                        configService.Settings.ServerUrl, configService.Settings.ServerPort).Result;

                    validConfiguration = validServer && validOfficeCode;
                }
            }

            if (!validConfiguration)
            {
                var view = new ChangeServerView();
                var vm = Locator.Instance.Resolve<ChangeServerViewModel>();
                vm.View = view;
                vm.ServerUrl = string.IsNullOrWhiteSpace(configService.Settings.ServerUrl) ? ChangeServerViewModel.DEFAULT_SERVER : configService.Settings.ServerUrl;
                vm.Port = string.IsNullOrWhiteSpace(configService.Settings.ServerPort) ? ChangeServerViewModel.DEFAULT_PORT : configService.Settings.ServerPort;
                vm.OfficeCode = configService.Settings.OfficeCode;
                vm.UseADAuthentication = configService.Settings.UseADAuthentication;
                view.DataContext = vm;
                view.Owner = null;
                view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                vm.ForceSave = true;
                var result = view.ShowDialog();
                if (result == true)
                {
                    configService.LoadSettings();
                    configService.Settings.ServerUrl = vm.ServerUrl;
                    configService.Settings.ServerPort = vm.Port;
                    configService.Settings.UseADAuthentication = vm.UseADAuthentication;
                    configService.Settings.OfficeCode = vm.OfficeCode;
                    configService.SaveSettings();
                }
            }



            MainViewModel appDataContext = null;
            try
            {
                appDataContext  = Locator.Instance.Resolve<MainViewModel>();
            }
            catch (Exception)
            {
                MessageBox.Show("Noah is not installed.\nPlease install Noah and try again.", "Error");
                Shutdown();
            }
            appDataContext.View = app;
            app.DataContext = appDataContext;

            var splashScreen = new SplashView();

            configService.LoadSettings();

            if (string.IsNullOrEmpty(configService.Settings.ServerUrl) ||
                string.IsNullOrEmpty(configService.Settings.ServerPort))
            {
                Shutdown();
            }

            var contextHelper = Locator.Instance.Resolve<ContextHelper>();
            if (contextHelper.CurrentUser == null)
            {
                var loginView = new LoginView();
                var vm = Locator.Instance.Resolve<LoginViewModel>();
                vm.View = loginView;
                loginView.DataContext = vm;
                loginView.ShowDialog();
            }

            if (contextHelper.CurrentUser == null)
            {
                Shutdown();
            }
            else
            {
                splashScreen.Show();
                app.Show();
                splashScreen.Close();
            }





        }

        

        static void UnhandledExceptionOccured(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            string message = "An unhandled error occurred: " + e.Message;
            message += Environment.NewLine + "Please contact TIMS Support.";
            // Show a message before closing application
            var dialogService = new MvvmDialogs.DialogService();
            dialogService.ShowMessageBox((INotifyPropertyChanged)(app.DataContext), message,  "Unhandled Error");
            
            Log.Write(LogEventLevel.Error, message);
        }
        
    }
}
