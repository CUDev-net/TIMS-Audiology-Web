using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class TaxGroupAssignment : Entity
    {
        public int UpdatedUserId { get; set; }
        public int ItemID { get; set; }
        public TaxItem TaxItem { get; set; }
        public TaxGroup TaxGroup { get; set; }
    }
}