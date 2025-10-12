using System;
using System.Collections.Generic;
using TIMS_X.BLL.VendorSync.Common;

namespace TIMS_X.BLL.VendorSync.Audigy
{
    public class Patient
    {
        public Patient()
        {
        }
        public string PracticeID { get; set; }
        public int PatientID { get; set; }
        public string AccountID { get; set; }
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }
        public string Salutation { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string WorkPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string FirstVisit { get; set; }
        public int AudiologistID { get; set; }
        public string AccountBalance { get; set; }
        public string ReferringPhysician { get; set; }
        public string MarketingSource { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public DateTime InsertDateTime { get; set; }
    }
}