using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIMS_X.BLL.VendorSync.Audigy
{
    public class HAUnitReturned
    {
        public string PracticeID { get; set; }
        public int PatientID { get; set; }
        public int AudiologistID { get; set; }
        public int TransactionID { get; set; }
        public int HAUnitID { get; set; }
        public string TransactionType { get; set; } = "Return";
        public string SerialNumber { get; set; }
        public string ReturnDate { get; set; }
        public string Side { get; set; }
        public string OriginalSaleDate { get; set; }
        public string ReturnReason { get; set; }
        public string ReturnAmount { get; set; }
        public string Notes { get; set; }
        public DateTime LastUpdatedDate { get; set; }

    }
}
