using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using TIMS_X.CloudAssistant.ViewModels;

namespace TIMS_X.CloudAssistant.Views
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class LoginView : MetroWindow
	{
		#region Constructors

		public LoginView()
		{
			InitializeComponent();
			Closing += LoginView_Closing;
			LogInButton.Focus();
		}

		#endregion Constructors

		#region LoginView Members

		public void ClearErrorMessages()
		{
			PasswordErrorMessage.Text = null;
		}

		public void SetPasswordError(string message)
		{
			PasswordErrorMessage.Text = message;
			PasswordErrorMessage.Visibility = Visibility.Visible;
			PasswordBox.Focus();
		}

		#endregion LoginView Members

		#region Private Members

		private void _HandleKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.F5)
			{
				var vm = (LoginViewModel)DataContext;
				vm.ToggleSupportLogin();
			}
		}

		private void _WindowLoaded(object sender, RoutedEventArgs e)
		{
			var vm = (LoginViewModel)DataContext;
			vm.WindowLoaded();
		}

		private void LoginView_Closing(object sender, CancelEventArgs e)
		{
		}

		#endregion Private Members
	}
}