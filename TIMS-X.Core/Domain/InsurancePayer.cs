using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class InsurancePayer : Entity, IUpdateAudited
    {
        public bool Inactive { get; set; }
        public int InUse { get; set; }
        public string Name { get; set; }
        public bool Protected { get; set; }
        public string OrganizationId { get; set; }
        public string ClaimOfficeNum { get; set; }
        public string InsuranceType { get; set; }
        public string ProviderId { get; set; }
        public string SecondaryId { get; set; }
        public string Contact { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Extension { get; set; }
        public string Fax { get; set; }
        public string Notes { get; set; }
        public string EmcProviderId { get; set; }
        public string SecondaryIdQualifier { get; set; }
        public string QbId { get; set; }
        public DateTime? DtQbModified { get; set; }
        public bool IsInstitutional { get; set; }
        public string CarrierCode { get; set; }
        public string Npi { get; set; }
        public bool PriCarrierCodeReq { get; set; }
        public bool IsIcd10 { get; set; }
        public string Email { get; set; }
        public bool AcceptsFaxAttachments { get; set; }
        public bool AcceptsEmailAttachments { get; set; }
        public bool AcceptsElectronicAttachments { get; set; }
        public bool MipsRequired { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}