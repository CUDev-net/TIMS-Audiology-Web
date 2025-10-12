using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TIMS_X.Core.Enums
{
    public enum OpportunityStatusEnum
    {
        /// <summary>
        /// Not set
        /// </summary>
        [Description("")]
        NotSet = 0,
        /// <summary>
        /// 2 Ears Aidable
        /// </summary>
        [Description("2 Ears Aidable")]
        TwoEarsAidable = 1,
        /// <summary>
        /// 2 Ears Aidable
        /// </summary>
        [Description("1 Ear Aidable")]
        OneEarAidable = 2,
        /// <summary>
        /// 2 Ears Aidable
        /// </summary>
        [Description("Re-sell 2 Ears")]
        ReSell2Ears = 3,
        /// <summary>
        /// 2 Ears Aidable
        /// </summary>
        [Description("Re-sell 1 Ear")]
        ReSell1Ear = 4,
        /// <summary>
        /// 2 Ears Aidable
        /// </summary>
        [Description("No Marketing")]
        NoMarketingOneEar = 5,
        /// <summary>
        /// No Opportunity
        /// </summary>
        [Description("No Opportunity")]
        NoOpportunity = 6,
        /// <summary>
        /// Tested Not Sold
        /// </summary>
        [Description("Tested Not Sold")]
        TestedNotSold = 7,
        /// <summary>
        /// Tested Not Sold 1 Ear
        /// </summary>
        [Description("Tested Not Sold 1 Ear")]
        TestedNotSold1Ear = 8,
        /// <summary>
        /// 2 Ears Aidable
        /// </summary>
        [Description("No Marketing")]
        NoMarketing = 9,

        /// <summary>
        /// Current User
        /// </summary>
        [Description("Current User")]
        CurrentUser = 10
    }
}
