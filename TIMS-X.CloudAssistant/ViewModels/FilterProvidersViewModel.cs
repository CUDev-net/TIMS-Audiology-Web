using System.Windows.Input;
using MvvmDialogs;
using MvvmHelpers;
using TIMS_X.CloudAssistant.Models;
using TIMS_X.CloudAssistant.Utils;
using TIMS_X.CloudAssistant.Views;

namespace TIMS_X.CloudAssistant.ViewModels
{
	public class FilterProvidersViewModel : ViewModelBase, IModalDialogViewModel
	{
		public FilterProvidersViewModel(ObservableRangeCollection<FilterProviderModel> providers)
		{
			ProviderList = new ObservableRangeCollection<FilterProviderModel>();
			foreach (var provider in providers)
			{
				var clone = new FilterProviderModel
				{
					IsSelected = provider.IsSelected,
					Provider = provider.Provider
				};
				ProviderList.Add(clone);
			}
		}

		public ICommand HandlesDeSelectAllCommand => new RelayCommand(() =>
		{
			foreach (var provider in ProviderList)
				provider.IsSelected = false;
		}, () => true);

		public ICommand HandlesOkayCommand => new RelayCommand(() =>
		{
			DialogResult = true;
			((FilterProvidersView)View).Close();
		}, () => true);

		public ICommand HandlesSelectAllCommand => new RelayCommand(() =>
		{
			foreach (var provider in ProviderList)
				provider.IsSelected = true;
		}, () => true);

		public ObservableRangeCollection<FilterProviderModel> ProviderList { get; set; }
		public bool? DialogResult { get; set; } = false;
	}
}