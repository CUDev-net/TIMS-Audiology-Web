using System;
using TIMS_X.Core.Domain.Base;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Domain
{
    public class PatientInsurance : Entity, ICreateDateAudited, IUpdateAudited
	{
        public int PatientId { get; set; }
        public PayerLevel PayerLevel { get; set; }
        public string RelationtoInsured { get; set; }
        public DateTime? PatientSignatureDate { get; set; }
        public int InsurancePayerId { get; set; }
        public string IdNumber { get; set; }
        public string PolicyGroupNum { get; set; }
        public string PolicyGroupName { get; set; }
        public string PolicyType { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string WorkPhone { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Sex { get; set; }
        public string EmploymentStatus { get; set; }
        public string Employer { get; set; }
        public DateTime? RetireDate { get; set; }
        public DateTime? SignatureDate { get; set; }
        public string InsurNotes { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public string ClaimFilingIndicator { get; set; }
        public bool InsuredAssignmentSig { get; set; }
        public string CarrierCode { get; set; }
        public DateTime UpdatedDate { get; set; }

        public InsurancePayer InsurancePayer { get; set; }
    }
}
