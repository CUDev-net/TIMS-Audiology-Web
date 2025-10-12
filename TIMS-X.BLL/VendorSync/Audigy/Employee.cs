using TIMS_X.BLL.VendorSync.Common;

namespace TIMS_X.BLL.VendorSync.Audigy
{
    public class Employee : DataSyncItem
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsAudiologist { get; set; }
        public int SiteId { get; set; }
    }
}