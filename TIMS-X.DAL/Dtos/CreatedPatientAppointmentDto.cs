using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.Dtos;

public class CreatedPatientAppointmentDto
{
	public Appointment Appointment { get; set; }
	public string EmailMessage { get; set; }
	public string Message { get; set; }
	public string PendingMessage { get; set; }
	public string PracticeMessage { get; set; }
}