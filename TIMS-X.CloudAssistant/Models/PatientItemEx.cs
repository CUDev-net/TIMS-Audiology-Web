using MvvmHelpers;
using TIMS_X.CloudAssistant.Utils;
using TIMS_X.Core.Models;

namespace TIMS_X.CloudAssistant.Models
{
	public class PatientItemEx : ObservableObject
	{
		public bool Inactive => Model.Inactive;

		public PatientItem Model { get; set; }

		public string Name => NameUtilities.FormatPatientName(Model.FirstName, Model.Initial, Model.LastName);

		public string NameAndDob => NameUtilities.FormatPatientName(Model.FirstName, Model.Initial, Model.LastName) +
		                            " " + Model.IdAndDob;

		public void Refresh()
		{
			OnPropertyChanged(nameof(Name));
			OnPropertyChanged(nameof(NameAndDob));
		}
	}

	public class ScheduledPatientItemEx : ObservableObject
	{
		public ScheduledPatientItem Model { get; set; }

		public string NameAndAppointmentTime =>
			NameUtilities.FormatPatientName(Model.FirstName, Model.Initial, Model.LastName) + " - " +
			Model.AppointmentTime;

		public void Refresh()
		{
			OnPropertyChanged(nameof(NameAndAppointmentTime));
		}
	}
}