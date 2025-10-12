using System;
using System.Collections.Generic;
using TIMS_X.BLL.VendorSync.Common;

namespace TIMS_X.BLL.VendorSync.Audigy;

public class PointOfSaleDocument : DataSyncItem
{
    public PointOfSaleDocument()
    {
        Lines = new List<PointOfSaleLine>();
    }

    public int ClinicId { get; set; }
    public int PatientId { get; set; }
    public int AppointmentId { get; set; }
    public DateTime Date { get; set; }
    public decimal Total { get; set; }
    public string PaymentType { get; set; }
    public decimal SalesTax { get; set; }
    public DateTime LastUpdate { get; set; }
    public List<PointOfSaleLine> Lines { get; set; }
}