using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Models
{
    public class NotificationResult
    {
        public int AppointmentId { get; set; }
        public MessageDeliveryMethod DeliveryMethod { get; set; }
        public DateTime? SentDate { get; set; }
        public string ErrorMessage { get; set; }

    }
}
