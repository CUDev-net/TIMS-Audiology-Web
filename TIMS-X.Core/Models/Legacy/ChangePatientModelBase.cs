using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Models.Legacy
{
    public class ChangePatientModelBase
    {
        #region ChangeAppointmentModelBase Members

        public string AccountNumber { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public DateTime? BirthDate { get; set; }

        public string City { get; set; }

        public int DefaultProviderID
        {
            get;
            set;
        }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string HomePhone { get; set; }

        public string LastName { get; set; }

        public int? MarketingReferenceId { get; set; }

        public string MiddleInitial { get; set; }

        public string MobilePhone { get; set; }

        public int? ReferralSourceId { get; set; }

        public string Sex { get; set; }

        public int SiteId { get; set; }

        public string SocialSecurityNumber { get; set; }

        public string State { get; set; }

        public string WorkPhone { get; set; }

        public string ZipCode { get; set; }

        #endregion ChangeAppointmentModelBase Members

    }
}
