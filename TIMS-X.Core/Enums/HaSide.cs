using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TIMS_X.Core.Enums
{
    public enum HaSide
    {
        /// <summary>
        /// Accessory item, no side
        /// </summary>
        [Description("Accessory")]
        Accessory,
        /// <summary>
        /// Right
        /// </summary>
        [Description("Right")]
        Right,
        /// <summary>
        /// Left
        /// </summary>
        [Description("Left")]
        Left,
        /// <summary>
        /// Left
        /// </summary>
        [Description("NA")]
        NotSet = 99
    }
}
