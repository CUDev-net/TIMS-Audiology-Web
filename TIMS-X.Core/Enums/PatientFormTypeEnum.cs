using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TIMS_X.Core.Enums
{
    public enum PatientFormTypeEnum
    {
        /// <summary>
        /// None
        /// </summary>
        [Description("No Form Selected")]
        None = 0,
        /// <summary>
        /// Patient Intake
        /// </summary>
        [Description("Patient Intake Form")]
        Intake = 1
    }
}
