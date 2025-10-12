using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class TaxItem : Entity, IUpdateDateAudited
    {
        public bool Inactive { get; set; }
        public int InUse { get; set; }
        public string QuickBookId { get; set; }
        public DateTime? DateQuickBookModified { get; set; }
        public int UpdatedUserId { get; set; }
        public string Name { get; set; }
        public bool Protected { get; set; }
        public string Description { get; set; }
        public int AgencyId { get; set; }
        public bool IsPercent { get; set; }
        public decimal Rate { get; set; }
        public TaxAgency TaxAgency { get; set; }
        public virtual ICollection<TaxGroup> TaxGroups { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}