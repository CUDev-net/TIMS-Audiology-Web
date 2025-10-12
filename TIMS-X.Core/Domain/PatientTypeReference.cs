using System;

namespace TIMS_X.Core.Domain
{
	public class PatientTypeReference 
	{
		public int PatientTypeId { get; set; }
		public int PatientId { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}
