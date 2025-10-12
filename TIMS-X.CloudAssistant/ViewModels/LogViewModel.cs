using MvvmDialogs;

namespace TIMS_X.CloudAssistant.ViewModels
{
	public class LogViewModel : ViewModelBase, IModalDialogViewModel
	{
		private string _logContents;

		public string LogContents
		{
			get => _logContents;
			set
			{
				_logContents = value;
				OnPropertyChanged();
			}
		}

		public bool? DialogResult => false;
	}
}