using System;

namespace TIMS_X.DAL.Dtos;

public class PatientScheduledAppointmentDto
{
	public DateTime BirthDate { get; set; }
	public DateTime Date { get; set; }
	public string Email { get; set; }
	public string FirstName { get; set; }
	public bool IsNewPatient { get; set; }
	public string LastName { get; set; }
	public string MiddleInitial { get; set; }
	public string Phone { get; set; }
	public int ProviderId { get; set; }
	public string Reason { get; set; }
	public int SiteId { get; set; }
}