using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
	public class MessageSettings : Entity, IUpdateAudited
	{
		public string AccountSid { get; set; }
		public int AppointmentReminderDayInterval { get; set; }
		public string AuthorizationToken { get; set; }
		public string FromEmailAddress { get; set; }
		public string FromPhoneNumber { get; set; }
		public string FromSmsNumber { get; set; }
		public DateTime? InitiateVoiceCallAt { get; set; }
		public bool IsBulkEmailEnabled { get; set; }
		public bool IsEFaxEnabled { get; set; }
		public bool IsEmailEnabled { get; set; }
		public bool IsSmsEnabled { get; set; }
		public bool IsVoiceEnabled { get; set; }
		public DateTime? SendEmailAt { get; set; }
		public DateTime? SendSmsAt { get; set; }
		public int? UpdatedUserId { get; set; }
		public DateTime UpdatedDate { get; set; }
	}
}