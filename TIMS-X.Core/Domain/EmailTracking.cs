using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain
{
    public class EmailTracking
    {
        public int Id { get; set; }
        public int EmailLogId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
