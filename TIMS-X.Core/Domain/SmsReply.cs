using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class SmsReply
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string Body { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Identifier { get; set; }
        public int SmsLogId { get; set; }
    }
}
