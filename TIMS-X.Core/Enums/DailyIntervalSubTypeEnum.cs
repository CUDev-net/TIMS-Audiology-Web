using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TIMS_X.Core.Enums
{
    public enum DailyIntervalSubTypeEnum
    {
        /// <summary>
        /// Every nth day
        /// </summary>
        [Description("NthDay")]
        NthDay = 1,

        /// <summary>
        /// Every weekday
        /// </summary>
        [Description("Weekday")]
        Weekday = 2,

        /// <summary>
        /// Every Monday, Wednesday, Friday
        /// </summary>
        [Description("Monday/Wednesday/Friday")]
        MonWedFri = 3,

        /// <summary>
        /// Every Tuesday, Thursday
        /// </summary>
        [Description("Tuesday/Thursday")]
        TuesThurs = 4
    }
}
