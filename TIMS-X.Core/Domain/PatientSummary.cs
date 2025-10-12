using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Utils;

namespace TIMS_X.Core.Domain
{
    public class PatientSummary : Entity
    {
#pragma warning disable CS8618
        public DateTime UpdatedDate { get; set; }
        public string PatientStatus { get; set; }
        public string PatientType { get; set; }
        public string LastName { get; set; }
        public bool Deceased { get; set; }
        public string FirstName { get; set; }
        public string Initial { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public int PrimaryPhone { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string OtherPhone { get; set; }
        public string MobilePhone { get; set; }
        public string PhoneToDisplay { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public int? SiteId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public OpportunityStatusEnum OtStatusId { get; set; }
        public int OtStatusDescriptionId { get; set; }
        public DateTime? DeathDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastAppointmentDate { get; set; }
        public string LastAppointmentStatus { get; set; }
		public DateTime? NextAppointmentDate { get; set; }
		public string NextAppointmentStatus { get; set; }
		public string CurrentLeftHearingAid { get; set; }
        public string CurrentRightHearingAid { get; set; }
        public List<AppointmentSummary> Appointments { get; set; }
        public List<CommunicationRestriction> CommunicationRestrictions { get; set; }
        public string UpdatedByUserName { get; set; }
        public string Opportunity => EnumUtilities.GetDescriptionFromEnum(OtStatusId);
        public string OpportunityDescription { get; set; }
        public bool Inactive { get; set; }
#pragma warning restore CS8618
    }
}