using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Models
{
    public class PatientNotificationContainer<TModel> where TModel : class, IPatientNotification
    {
        #region MessageData Members

        public TModel Model { get; set; }

        public string ToEmail { get; set; }

        public string ToSms { get; set; }

        public string ToVoice { get; set; }

        #endregion MessageData Members

    }
}
