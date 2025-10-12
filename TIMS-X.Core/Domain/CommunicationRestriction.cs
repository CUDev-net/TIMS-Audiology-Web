using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class CommunicationRestriction : Entity, IUpdateAudited
    {
        public static readonly string NO_MAIL = "No Mail";
        public static readonly string NO_MARKETING = "No Marketing";
        public static readonly string NO_MARKETING_1EAR = "No Marketing 1 Ear";
        public static readonly string NOT_AT_THIS_TIME = "Not at this time";
        public string Name { get; set; }
        public string Description { get; set; }
        public int? InUse { get; set; }
        public bool Inactive { get; set; }
        public bool Protected { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
    }
}