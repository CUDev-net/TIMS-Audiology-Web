using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class PracticeSummary : Entity
    {
        public bool Inactive { get; set; }
        public string Name { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string EmailDisclaimer { get; set; }
        public string OfficeCode { get; set; }
        public string TaxId { get; set; }
        public bool UseBlockScheduling { get; set; }
        public bool UsesNoahDataMining { get; set; }
        public bool UsesAdAuthentication { get; set; }
        public bool LinkAppointmentHistory { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZipCode { get; set; }
        public string BillingPhoneNumber { get; set; }
        public bool UseSiteAddressForReports { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Locale { get; set; }
    }
}