using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class KpiSiteTarget : Entity, ICreateDateAudited, IUpdateDateAudited
    {
        public bool Inactive { get; set; }
        public int? Length { get; set; }
        public int SiteId { get; set; }
        public string Notes { get; set; }
        public decimal? TargetAmount1 { get; set; }
        public decimal? TargetAmount2 { get; set; }
        public Site Site { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}