using System;

namespace TIMS_X.Core.Domain
{
	public class AuthorizationReference 
	{
		public int AuthorizationId { get; set; }
		public int PatientId { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}
