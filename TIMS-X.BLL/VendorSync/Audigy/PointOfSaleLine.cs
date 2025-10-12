using System;
using TIMS_X.BLL.VendorSync.Common;

namespace TIMS_X.BLL.VendorSync.Audigy
{
    public class PointOfSaleLine : DataSyncItem
    {
        public DateTime? OrderDate { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string SerialNumber { get; set; }
        public string Side { get; set; }
        public string Manufacturer { get; set; }
        public string Style { get; set; }
        public string HaType { get; set; }
        public string Model { get; set; }
        public string BatterySize { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NetPrice { get; set; }
    }
}