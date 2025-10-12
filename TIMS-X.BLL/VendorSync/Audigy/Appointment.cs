using System;
using TIMS_X.BLL.VendorSync.Common;

namespace TIMS_X.BLL.VendorSync.Audigy;

public class Appointment : DataSyncItem
{
    public string PracticeID { get; set; }
    public int AppointmentID { get; set; }
    public int PatientID { get; set; }
    public DateTime DateTime { get; set; }
    public int Length { get; set; }
    public int AudiologistID { get; set; }
    public string AppointmentType { get; set; }
    public string AppointmentStatus { get; set; }
    public DateTime DateCreated { get; set; }
    public int EarsFit { get; set; }
    public decimal TotalAmount { get; set; }
    public string TechnologySold { get; set; }
    public string HAUserType { get; set; }
    public bool Has3rdParty { get; set; }
    public string Notes { get; set; }
    public DateTime LastUpdatedDate { get; set; }
    public int LocationID { get; set; }
    public DateTime InsertDateTime { get; set; }
    public int AppointmentTypeID { get; set; }
}