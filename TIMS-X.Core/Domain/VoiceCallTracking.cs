using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain
{
    public class VoiceCallTracking
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }

        public string DigitsPressed { get; set; }

        public string Status { get; set; }

        public int VoiceCallLogId { get; set; }
    }
}
