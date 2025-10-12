using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class MarketingReferenceSite : Entity
    {
        public int MarketingReferenceId { get; set; }
        public int SiteId { get; set; }
    }
}
