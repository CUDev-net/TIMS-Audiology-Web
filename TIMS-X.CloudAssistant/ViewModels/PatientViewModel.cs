using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;

namespace TIMS_X.CloudAssistant.ViewModels
{
	public class PatientViewModel : ViewModelBase
	{
		private PatientItem _model;

		public PatientItem Model
		{
			get => _model;
			set
			{
				_model = value;
				OnPropertyChanged();
			}
		}

		public string PatientAvatarPath => Model?.Gender == Gender.Female
			? "../Assets/female_patient.png"
			: "../Assets/male_patient.png";

		public string PatientName { get; set; }


		public bool SaveChanges()
		{
			return true;
		}
	}
}