using System;
using TIMS_X.BLL.VendorSync.Common;

namespace TIMS_X.BLL.VendorSync.Audigy
{
    public class Audiologist
    {
        public string PracticeId { get; set; }
        public int AudiologistId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public DateTime InsertDateTime { get; set; }
    }
}