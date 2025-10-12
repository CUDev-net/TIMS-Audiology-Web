using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class SmsLog : ICreateByUserAudited, ICreateDateAudited
    {
        public int Id { get; set; }
        public int? MessageTemplateId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ExceptionMessage { get; set; }
        public bool ExceptionOccurred { get; set; }
        public int AppointmentId { get; set; }
        public string Identifier { get; set; }
        public int NumberOfMessages { get; set; }
        public decimal MessageCost { get; set; }
        public bool IsPreview { get; set; }
        public int PatientId { get; set; }
        public MessageTemplate MessageTemplate { get; set; }
    }
}
