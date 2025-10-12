using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain
{
    public class VoiceCallLog
    {
        public int Id { get; set; }
        public int MessageTemplateId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string CallScript { get; set; }
        public DateTime CreatedDate { get; set; }
        public int AppointmentId { get; set; }
        public string ExceptionMessage { get; set; }
        public bool ExceptionOccurred { get; set; }
        public string Identifier { get; set; }
        public bool IsPreview { get; set; }

        public MessageTemplate MessageTemplate { get; set; }
    }
}
