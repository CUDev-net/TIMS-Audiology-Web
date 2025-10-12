using System.Windows.Controls;

namespace TIMS_X.CloudAssistant.Views
{
	/// <summary>
	///     Interaction logic for PatientView.xaml
	/// </summary>
	public partial class PatientView : UserControl
	{
		public PatientView()
		{
			InitializeComponent();
		}

		public StackPanel ModuleContainer => PART_ModuleContainer;

		public StackPanel SessionContainer => PART_SessionContainer;
	}
}