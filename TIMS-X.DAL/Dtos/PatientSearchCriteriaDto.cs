using System;

namespace TIMS_X.DAL.Dtos;

public class PatientSearchCriteriaDto
{
	public DateTime? DateOfBirth { get; set; }
	public string Email { get; set; }
	public string FirstName { get; set; }
	public bool IncludeInactive { get; set; }
	public string Initial { get; set; }
	public string LastName { get; set; }
	public string PhoneNumber { get; set; }
}