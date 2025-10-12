using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain
{
    public class SmsTracking
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }

        public int SmsLogId { get; set; }

        public string Status { get; set; }
    }
}
