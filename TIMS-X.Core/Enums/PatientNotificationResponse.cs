using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Enums
{
    public enum PatientNotificationResponse
    {
        Unknown = 0,
        Confirm = 1,
        Reschedule = 2,
        Cancel = 3,
        Repeat = 4
    }
}
