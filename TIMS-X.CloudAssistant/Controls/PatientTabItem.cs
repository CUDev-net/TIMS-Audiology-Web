using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TIMS_X.CloudAssistant.Controls
{
	public class PatientTabItem : TabItem
	{
		public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register("CloseCommand",
			typeof(ICommand), typeof(PatientTabItem), new PropertyMetadata(null));

		public static readonly DependencyProperty PatientIdProperty = DependencyProperty.Register("PatientId",
			typeof(int), typeof(PatientTabItem), new PropertyMetadata(null));

		public ICommand CloseCommand
		{
			get => (ICommand)GetValue(CloseCommandProperty);
			set => SetValue(CloseCommandProperty, value);
		}

		public int PatientId
		{
			get => (int)GetValue(PatientIdProperty);
			set => SetValue(PatientIdProperty, value);
		}
	}
}