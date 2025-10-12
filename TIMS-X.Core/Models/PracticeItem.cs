using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain;

namespace TIMS_X.Core.Models
{
    public class PracticeItem
    {
        public PracticeItem() { }
        public PracticeItem(Practice practice, List<SiteItem> sites)
        {
            Name = practice.Name;
            OwnerFirstName = practice.FirstName;
            OwnerLastName = practice.LastName;
            Fax = practice.Fax;
            Email = practice.Email;
            OfficeCode = practice.OfficeCode;
            BillingAddress1 = practice.BillingAddress1;
            BillingAddress2 = practice.BillingAddress2;
            BillingCity = practice.BillingCity;
            BillingState = practice.BillingState;
            BillingZipCode = practice.BillingZipCode;
            BillingPhoneNumber = practice.BillingPhoneNumber;
            Sites = sites;
        }
        public string Name { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string OfficeCode { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZipCode { get; set; }
        public string BillingPhoneNumber { get; set; }

        public List<SiteItem> Sites { get; set; }
    }
}
