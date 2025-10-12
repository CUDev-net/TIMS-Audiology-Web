using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class MarketingReference : Entity, IUpdateAudited
    {
        public MarketingReference()
        {
            Sites = new HashSet<MarketingReferenceSite>();
        }

        public bool Inactive { get; set; }
        public int InUse { get; set; }
        public string Name { get; set; }
        public bool Protected { get; set; }
        public int CategoryId { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Notes { get; set; }
        public DateTime? ReviewDate { get; set; }
        public ICollection<MarketingReferenceSite> Sites { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}