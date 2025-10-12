using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using TIMS_X.CloudAssistant.ViewModels;

namespace TIMS_X.CloudAssistant.Views
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : MetroWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			Closing += MainView_Closing;
		}

		public StackPanel NoahTestingPanel => PART_NoahTesting;

		private void _OnStateChanged(object sender, EventArgs e)
		{
			var viewModel = (MainViewModel)DataContext;
			viewModel?.OnWindowStateChanged(WindowState);
		}

		private void _WindowClosing(object sender, CancelEventArgs e)
		{
			var viewModel = (MainViewModel)DataContext;
			viewModel?.OnWindowClosing();
		}

		private void _WindowLoaded(object sender, RoutedEventArgs e)
		{
			var viewModel = (MainViewModel)DataContext;
			viewModel.OnWindowLoaded();
		}

		private void MainView_Closing(object sender, CancelEventArgs e)
		{
			/*
			    if (((MainViewModel)(this.DataContext)).Data.IsModified)
			    if (!((MainViewModel)(this.DataContext)).PromptSaveBeforeExit())
			    {
			        e.Cancel = true;
			        return;
			    }
			*/
		}

		private void OnRecentPatientDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var viewModel = (MainViewModel)DataContext;
			viewModel?.HandleSelectedRecentPatientDoubleClicked();
		}

		private void OnSearchPatientDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var viewModel = (MainViewModel)DataContext;
			viewModel?.HandleSelectedSearchPatientDoubleClicked();
		}

		private void OnTodayPatientDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var viewModel = (MainViewModel)DataContext;
			viewModel?.HandleSelectedTodayPatientDoubleClicked();
		}


		public void TogglePatientFlyout(bool? isOpen = null)
		{
			if (isOpen.HasValue)
				PatientFlyout.IsOpen = isOpen.Value;
			else
				PatientFlyout.IsOpen = !PatientFlyout.IsOpen;
		}
	}
}