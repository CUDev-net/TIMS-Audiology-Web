using System.ComponentModel;
using System.Windows;
using TIMS_X.CloudAssistant.ViewModels;

namespace TIMS_X.CloudAssistant.Views
{
	/// <summary>
	///     Interaction logic for About.xaml
	/// </summary>
	public partial class ChangeServerView : Window
	{
		public ChangeServerView()
		{
			InitializeComponent();
		}

		public void SetError(string errorMessage)
		{
			ErrorMessage.Text = errorMessage;
			ErrorMessage.Visibility = string.IsNullOrWhiteSpace(errorMessage)
				? Visibility.Collapsed
				: Visibility.Visible;
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			var vm = DataContext as ChangeServerViewModel;
			if (vm != null) e.Cancel = !vm.CanCloseWithoutSave();
		}
	}
}