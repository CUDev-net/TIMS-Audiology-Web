using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MvvmDialogs;
using MvvmHelpers;

namespace TIMS_X.CloudAssistant.ViewModels
{
	/// <summary>
	///     Implements INotifyPropertyChanged for all ViewModel
	/// </summary>
	public abstract class ViewModelBase : BaseViewModel
	{
		protected ViewModelBase()
		{
			Title = "TIMS Assistant";
		}

		public object View { get; set; }

		/// <summary>
		///     Executes Dispatcher.Invoke with the supplied Action after delayMs has passed.
		/// </summary>
		/// <param name="delayMs"></param>
		/// <param name="action"></param>
		public void DelayedDispatch(int delayMs, Action<object> action)
		{
			var window = (MetroWindow)View;
			if (window != null)
				Task.Delay(delayMs).ContinueWith(t => { window.Dispatcher.Invoke(action, this); });
		}

		public virtual void OnPrivacyModeChanged()
		{
		}

		protected MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button)
		{
			var dialogService = Locator.Instance.Resolve<IDialogService>();
			var window = (MetroWindow)View;
			var viewModel = (INotifyPropertyChanged)window.DataContext;
			return dialogService.ShowMessageBox(viewModel, message, caption, button);
		}
	}
}