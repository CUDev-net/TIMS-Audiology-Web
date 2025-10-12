using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TIMS_X.Core.Enums
{
    public enum OpportunityTrackingType
    {
        /// <summary>
        /// None
        /// </summary>
        [Description("None")]
        None,
        /// <summary>
        /// Active
        /// </summary>
        [Description("Active")]
        Active,
        /// <summary>
        /// ActiveWithThreshold
        /// </summary>
        [Description("Active w/Threshold")]
        ActiveWithThreshold
    }
}
