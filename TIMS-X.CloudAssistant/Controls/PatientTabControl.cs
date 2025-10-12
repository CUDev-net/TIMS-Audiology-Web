using System.Windows;
using System.Windows.Controls;

namespace TIMS_X.CloudAssistant.Controls
{
	public class PatientTabControl : TabControl
	{
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new PatientTabItem();
		}
	}
}