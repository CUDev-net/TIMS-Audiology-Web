using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain
{
    public class EmailLog
    {
        public int Id { get; set; }
        public int MessageTemplateId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string BodyText { get; set; }
        public string BodyHtml { get; set; }
        public int PatientId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ExceptionMessage { get; set; }
        public bool ExceptionOccurred { get; set; }
        public int AppointmentId { get; set; }
        public string Identifier { get; set; }

        public MessageTemplate MessageTemplate { get; set; }
    }
}
