using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TIMS_X.Core.Enums
{
    public enum IntervalTypeEnum
    {
        /// <summary>
        /// Daily
        /// </summary>
        [Description("Daily")]
        Daily = 1,
        /// <summary>
        /// Weekly
        /// </summary>
        [Description("Weekly")]
        Weekly = 2,
        /// <summary>
        /// Monthly
        /// </summary>
        [Description("Monthly")]
        Monthly = 3,
        /// <summary>
        /// Yearly
        /// </summary>
        [Description("Yearly")]
        Yearly = 4
    }
}
