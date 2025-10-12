using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TIMS_X.Core.Enums
{
    public enum MessageTemplateType
    {
        ///<summary>
        ///</summary>
        [Description("")]
        Unknown,
        ///<summary>
        ///</summary>
        [Description("Appointment Confirmation")]
        AppointmentConfirmation,
        ///<summary>
        ///</summary>
        [Description("Appointment Verification")]
        AppointmentVerification
    }
}
