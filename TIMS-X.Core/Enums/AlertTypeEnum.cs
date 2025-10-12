using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TIMS_X.Core.Enums
{
    public enum AlertTypeEnum
    {
        /// <summary>
        /// TIMSUserTask
        /// </summary>
        [Description("Task")]
        Task = 1,

        /// <summary>
        /// Appointment
        /// </summary>
        [Description("Appointment")]
        Appointment = 2,

        /// <summary>
        /// Appointment
        /// </summary>
        [Description("Custom")]
        Custom = 3,

        /// <summary>
        /// Appointment
        /// </summary>
        [Description("Task Note")]
        TaskNote = 4,
        /// <summary>
        /// Appointment
        /// </summary>
        [Description("Message Received")]
        MessageReceived = 5,
        
        /// <summary>
        /// Intake Sheet
        /// </summary>
        [Description("Intake Sheet Received")]
        IntakeSheetReceived = 6
    }
}
