using MvvmHelpers;
using TIMS_X.Core.Models;

namespace TIMS_X.CloudAssistant.Models
{
	public class FilterProviderModel : ObservableObject
	{
		private bool _isSelected;

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged();
			}
		}

		public ProviderItem Provider { get; set; }
	}
}