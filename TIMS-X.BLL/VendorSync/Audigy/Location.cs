using System;
using TIMS_X.BLL.VendorSync.Common;

namespace TIMS_X.BLL.VendorSync.Audigy
{
    public class Location : DataSyncItem
    {
        public string PracticeId { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
        public string Active { get; set; }
        public string MonStart {get; set;}
        public string MonEnd {get; set;}
        public string TuesStart {get; set;}
        public string TuesEnd {get; set;}
        public string WedStart {get; set;}
        public string WedEnd {get; set;}
        public string ThurStart {get; set;}
        public string ThurEnd {get; set;}
        public string FriStart {get; set;}
        public string FriEnd {get; set;}
        public string SatStart {get; set;}
        public string SatEnd {get; set;}
        public string SunStart {get; set;}
        public string SunEnd {get; set;}
    }
}