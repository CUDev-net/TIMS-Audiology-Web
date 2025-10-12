using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
	public class ApptAuthorization : Entity, IUpdateAudited
	{
		public int? Authorizations { get; set; }
		public DateTime? Expires { get; set; }
		public bool Inactive { get; set; }
		public bool IsDeleted { get; set; }
		public string Name { get; set; }
		public string Notes { get; set; }
		public int NumberUsed { get; set; }
		public string DisplayString { get; set; }
		public int PatientId { get; set; }
		public int? UpdatedUserId { get; set; }
		public DateTime UpdatedDate { get; set; }
	}
}