using System;

namespace TIMS_X.DAL.Dtos
{
	public class PatientCandidateDto
	{
		public DateTime? DateLastUpdated { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string Initial { get; set; }
		public string LastName { get; set; }
		public string Phone { get; set; }
		public long PatientId { get; set; }
	}
}