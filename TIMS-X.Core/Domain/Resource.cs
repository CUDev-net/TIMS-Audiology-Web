using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class Resource : Entity, IUpdateAudited
    {
        public bool Inactive { get; set; }
        public int InUse { get; set; }
        public string Name { get; set; }
        public int SiteId { get; set; }
        public string Description { get; set; }
        public int? Color { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}