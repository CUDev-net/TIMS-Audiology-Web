using System;
using TIMS_X.BLL.VendorSync.Common;

namespace TIMS_X.BLL.VendorSync.Audigy
{
    public class PatientInsurance : DataSyncItem
    {
        public Patient Patient { get; set; }
        public InsuranceCompany InsuranceCompany { get; set; }
        public string RelationshipToPatient { get; set; }
        public string GroupNumber { get; set; }
        public string PlanName { get; set; }
        public string PolicyHolder { get; set; }
        public decimal CoPay { get; set; }
        public decimal Deductible { get; set; }
    }
}