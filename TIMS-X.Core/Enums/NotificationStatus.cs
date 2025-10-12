using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Enums
{
    [Flags]
    public enum NotificationStatus
    {
        /// <summary>
        /// No notifications have been sent for this appointment
        /// </summary>
        NothingSent = 0,
        /// <summary>
        /// Email confirmation sent
        /// </summary>
        EmailConfirmationSent = 1,
        /// <summary>
        /// SMS confirmation sent
        /// </summary>
        SmsConfirmationSent = 2,
        /// <summary>
        /// Voice confirmation sent
        /// </summary>
        VoiceConfirmationSent = 4,
        /// <summary>
        /// Email reminder sent
        /// </summary>
        EmailReminderSent = 8,
        /// <summary>
        /// SMS reminder sent
        /// </summary>
        SmsReminderSent = 16,
        /// <summary>
        /// Voice reminder sent
        /// </summary>
        VoiceReminderSent = 32,
    }
}
