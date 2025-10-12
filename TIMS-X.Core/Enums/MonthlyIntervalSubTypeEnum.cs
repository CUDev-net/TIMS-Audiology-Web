using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TIMS_X.Core.Enums
{
    public enum MonthlyIntervalSubTypeEnum
    {
        /// <summary>
        /// Which day of the month.
        /// </summary>
        [Description("DayOfMonth")]
        DayOfMonth = 1,

        /// <summary>
        /// Which day of the week.
        /// </summary>
        [Description("DayOfWeek")]
        DayOfWeek = 2,
    }
}
