using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIMS_X.BLL.VendorSync.Audigy
{
    public class HAUnitSold
    {
        public string PracticeID { get; set; }
        public int PatientID { get; set; }
        public int AudiologistID { get; set; }
        public int TransactionID { get; set; }
        public int HAUnitID { get; set; }
        public string TransactionType { get; set; } = "Sale";
        public string PurchaseDate { get; set; }
        public string Side { get; set; }
        public string SerialNumber { get; set; }
        public string WarrantyDate { get; set; }
        public string HATechnology { get; set; }
        public string ManufacturerName { get; set; }
        public string OrderDate { get; set; }
        public string BatterySize { get; set; }
        public string UnitPrice { get; set; }
        public string Discount { get; set; }
        public string PurchasePrice { get; set; }
        public string Notes { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public DateTime InsertDateTime { get; set; }
        public string AppointmentID { get; set; }
    }
}
