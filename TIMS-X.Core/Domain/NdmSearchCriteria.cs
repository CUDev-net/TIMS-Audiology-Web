using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain
{
    public class NdmSearchCriteria
    {
        public NdmSearchCriteria()
        {
            SearchPoints = new HashSet<NdmSearchPoint>();
        }

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByUserId { get; set; }
        public int UpdatedByUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Inactive { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPureTone { get; set; }
        public bool IsMcl { get; set; }
        public bool IsUcl { get; set; }
        public bool UsedForOpportunityTracking { get; set; }
        public int? TypeofLossId { get; set; }
        public int? SeverityId { get; set; }
        public int? SeverityMinPoints { get; set; }
        public bool IsBc { get; set; }
        public bool IsBaha { get; set; }
        public int? AirBoneGap { get; set; }

        public ICollection<NdmSearchPoint> SearchPoints { get; set; }
    }
}
