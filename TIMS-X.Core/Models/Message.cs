using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Models
{
    public class Message
    {
        public string Text { get; set; }
        public bool FromPatient { get; set; }
        public DateTime DateSent { get; set; }
        public int UserId { get; set; }
    }
}
