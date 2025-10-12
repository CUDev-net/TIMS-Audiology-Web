using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Domain
{
    public class Patient : AuditableEntity, ICreateByUserAudited, IUpdateAudited, ISiteAuditable, ICreateDateAudited
    {
        public Patient()
        {
            Restrictions = new HashSet<PatientRestriction>();
            MedicalConditions = new HashSet<PreviousHistory>();
            AuthorizationReferences = new HashSet<AuthorizationReference>();
		}

        public Guid Guid { get; set; }
        public bool Inactive { get; set; }
        public int? SalutationId { get; set; }
        public int? ProviderId { get; set; }
        public int? PrimaryCareId { get; set; }
        public int? ReferringPhysicianId { get; set; }
        public int? MarketingId { get; set; }
        public int? SiteId { get; set; }
        public string MaritalStatusId { get; set; }
        public string EmplStatusId { get; set; }
        public int? PatientStatusId { get; set; }
        public int? PatientTypeId { get; set; }
        public int OtStatusId { get; set; }
        public int OtStatusDescriptionId { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Initial { get; set; }
        public string PreferredName { get; set; }
        public string Sex { get; set; }
        public bool Deceased { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public PrimaryPhoneEnum PrimaryPhone { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string WorkPhone { get; set; }
        public string OtherPhone { get; set; }
        public string AccountNo { get; set; }
        public string Notes { get; set; }
        public string CustomText1 { get; set; }
        public string CustomText2 { get; set; }
        public DateTime? CustomDate1 { get; set; }
        public DateTime? CustomDate2 { get; set; }
        public string Ssn { get; set; }
        public int? InsuredInsurancePayerId { get; set; }
        public int? UpdatedSiteId { get; set; }
        public RaceEnum? Race { get; set; }
        public EthnicityEnum? Ethnicity { get; set; }
        public LanguageEnum? Language { get; set; }
        public bool LegalRep { get; set; } // AKA Name and Address for Invoicing
		public string LegalRepFirstName { get; set; }
        public string LegalRepLastName { get; set; }
        public string LegalRepInitial { get; set; }
        public string LegalRepAddress1 { get; set; }
        public string LegalRepAddress2 { get; set; }
        public string LegalRepCity { get; set; }
        public string LegalRepState { get; set; }
        public string LegalRepZipCode { get; set; }
        public string LegalRepPhone { get; set; }
        public string AlternateContact { get; set; }
        public string AlternateContactPhone { get; set; }
        public bool ReleaseSignature { get; set; }
        public DateTime? ReleaseSignatureDate { get; set; }
        public bool AssignBenefits { get; set; }
        public DateTime? AssignBenefitsDate { get; set; }
        public string ResponsibleParty { get; set; }
        public bool HasIntakeData { get; set; }

        public string LastFirstMiddle => LastName + ", " + FirstName +
                                         (string.IsNullOrEmpty(Initial) ? string.Empty : " " + Initial + ".");
        public string FirstLast => FirstName + " " + LastName;

        public string SexFull
        {
            get
            {
                if (Sex == "M") return "Male";
                if (Sex == "F") return "Female";
                return "Unknown";
            }
        }

        public Salutation Salutation { get; set; }
        public Provider Provider { get; set; }
        public MarketingReference PrimaryCarePhysician { get; set; }
        public MarketingReference ReferringPhysician { get; set; }
        public MarketingReference Marketing { get; set; }
        public User UpdatedByUser { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? PrivacyDate { get; set; }
		public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool UseSecondaryAddress { get; set; }
        public string SecondaryAddress1 { get; set; }
        public string SecondaryAddress2 { get; set; }
        public string SecondaryCity { get; set; }
        public string SecondaryState { get; set; }
        public string SecondaryZipCode { get; set; }
        public string QBID { get; set; }
        public bool ReleaseInformation { get; set; }
        public DateTime? ReleaseInformationDate { get; set; }
        public DateTime? ConsentDate { get; set; }
        public bool MarketingAuthorization { get; set; }
        public DateTime? MarketingAuthorizationDate { get; set; }
        public string AuthorizedParties { get; set; }
        public string StudentStatusId { get; set; }
		public ICollection<PatientRestriction> Restrictions { get; set; }
        public ICollection<PreviousHistory> MedicalConditions { get; set; }
        public int[] PatientTypeIds { get; set; }
        public int[] RestrictionIds { get; set; }
        public int[] AuthorizationIds { get; set; }
        public ICollection<PatientTypeReference> PatientTypeReferences { get; set; }
        public ICollection<AuthorizationReference> AuthorizationReferences { get; set; }

		//public ICollection<PatientInsurance> PatientInsurances { get; set; }

	}
}