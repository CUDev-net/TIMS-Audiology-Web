using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class ReferralSource : IUpdateAudited
    {
        public int Id { get; set; }
        public bool Inactive { get; set; }
        public int InUse { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int SalutationId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Initial { get; set; }
        public string Practice { get; set; }
        public string Degree { get; set; }
        public string PhysicianId { get; set; }
        public string IdType { get; set; }
        public string NetworkId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int? PostalRegion { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Ext { get; set; }
        public string Fax { get; set; }
        public string Notes { get; set; }
        public DateTime? AgreementDate { get; set; }
        public string Npi { get; set; }
        public string TaxId { get; set; }
        public string TaxIdType { get; set; }
        public string SecondaryIdNum { get; set; }
        public string SecondaryIdQualifier { get; set; }
        public int MktReferenceId { get; set; }
        public string Email { get; set; }
        public string SecureEmail { get; set; }
    }
}
