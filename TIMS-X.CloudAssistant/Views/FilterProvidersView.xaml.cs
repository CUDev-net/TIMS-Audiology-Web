using System.Windows;
using System.Windows.Controls;

namespace TIMS_X.CloudAssistant.Views
{
	/// <summary>
	///     Interaction logic for About.xaml
	/// </summary>
	public partial class FilterProvidersView : Window
	{
		public FilterProvidersView()
		{
			InitializeComponent();
		}

		public ListBox ProviderListBox => _providerListBox;
	}
}