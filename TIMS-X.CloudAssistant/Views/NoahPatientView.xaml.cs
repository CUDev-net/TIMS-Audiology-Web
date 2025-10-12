using System.Windows.Controls;

namespace TIMS_X.CloudAssistant.Views
{
	/// <summary>
	///     Interaction logic for PatientView.xaml
	/// </summary>
	public partial class NoahPatientView : UserControl
	{
		public NoahPatientView()
		{
			InitializeComponent();
		}

		public Grid ModuleContainer => PART_ModuleContainer;

		public Grid SessionContainer => PART_SessionContainer;
	}
}