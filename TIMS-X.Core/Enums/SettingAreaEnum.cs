using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TIMS_X.Core.Enums
{
    public enum SettingAreaEnum
    {
        /// <summary>
        /// Patient
        /// </summary>
        [Description("Patient")]
        Patient,
        /// <summary>
        /// 
        /// </summary>
        [Description("History")]
        History,
        /// <summary>
        /// 
        /// </summary>
        [Description("Hearing Aid History")]
        HAHistory,
        /// <summary>
        /// 
        /// </summary>
        [Description("Data Analysis")]
        Dashboard,
        /// <summary>
        /// 
        /// </summary>
        [Description("Imaging")]
        Imaging,
        /// <summary>
        /// 
        /// </summary>
        [Description("Superbill")]
        Superbill,
        /// <summary>
        /// 
        /// </summary>
        [Description("Appointment")]
        Appointment,
        /// <summary>
        /// 
        /// </summary>
        [Description("Claims")]
        Claims,
        /// <summary>
        /// 
        /// </summary>
        [Description("General")]
        General,
        /// <summary>
        /// 
        /// </summary>
        [Description("Point Of Sale")]
        POS,
        /// <summary>
        /// 
        /// </summary>
        [Description("Quickbooks")]
        QB,
        /// <summary>
        /// 
        /// </summary>
        [Description("Reports")]
        Reports,
        /// <summary>
        /// 
        /// </summary>
        [Description("Centers")]
        SystemCenter,
        /// <summary>
        /// 
        /// </summary>
        [Description("Communications")]
        Marketing,
        /// <summary>
        /// 
        /// </summary>
        [Description("Financing")]
        Financing,
        /// <summary>
        /// 
        /// </summary>
        [Description("Setup")]
        Setup,
        /// <summary>
        /// 
        /// </summary>
        [Description("Tasks")]
        Tasks,
        /// <summary>
        /// 
        /// </summary>
        [Description("HL7")]
        HL7
    }
}
