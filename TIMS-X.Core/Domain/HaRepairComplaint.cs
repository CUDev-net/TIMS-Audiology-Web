using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class HaRepairComplaint : Entity, IUpdateAudited
    {
        public bool Inactive { get; set; }
        public int? InUse { get; set; }
        public string Name { get; set; }
        public bool Protected { get; set; }
        public string Description { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}