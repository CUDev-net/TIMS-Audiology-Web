using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Models
{
    public class PatientMessageModel
    {
        #region PatientSearchModel Members
        public int PatientId { get; set; }
        public MessageDeliveryMethod DeliveryMethod { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string EmailSubject { get; set; }
        public string Message { get; set; }

        #endregion PatientSearchModel Members

    }
}
