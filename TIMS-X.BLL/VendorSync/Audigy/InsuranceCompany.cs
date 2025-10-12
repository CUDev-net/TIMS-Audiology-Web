using TIMS_X.BLL.VendorSync.Common;

namespace TIMS_X.BLL.VendorSync.Audigy
{
    public class InsuranceCompany : DataSyncItem
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
    }
}