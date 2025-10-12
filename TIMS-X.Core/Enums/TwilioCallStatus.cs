using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Enums
{
    public enum TwilioCallStatus
    {
        /// <summary>
        /// The call is ready and waiting in line before going out.
        /// </summary>
        Queued,

        /// <summary>
        /// The call is currently ringing.
        /// </summary>
        Ringing,

        /// <summary>
        /// The call was answered and is currently in progress.
        /// </summary>
        InProgress,

        /// <summary>
        /// The call was answered and has ended normally.
        /// </summary>
        Completed,

        /// <summary>
        /// The caller received a busy signal.
        /// </summary>
        Busy,

        /// <summary>
        /// The call could not be completed as dialed, most likely because the phone number was non-existent.
        /// </summary>
        Failed,

        /// <summary>
        /// The call ended without being answered.
        /// </summary>
        NoAnswer
    }
}
