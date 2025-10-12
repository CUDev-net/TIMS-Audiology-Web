using System;
using System.Collections;
using System.Collections.Generic;
using TIMS_X.Core.Attributes;
using TIMS_X.Core.Domain.Base;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models.Api;
using TIMS_X.Core.Models.Legacy;
using TIMS_X.Core.Utils;

namespace TIMS_X.Core.Domain
{
    public class Appointment: Entity, IUpdateAudited, ICreateByUserAudited, ICreateDateAudited
    {
        public Appointment()
        {
            PosDocuments = new HashSet<PosDocument>();
        }

        public Appointment(CreateAppointmentModel createModel)
        {
            PatientId = createModel.PatientId;
            AppointmentTypeId = createModel.AppointmentTypeId;
            BillToProviderId = createModel.ProviderId;
            ProviderId = createModel.ProviderId;
            SiteId = createModel.SiteId;
            StartsAt = createModel.StartsAt;
            EndsAt = createModel.EndsAt;
            Notes = createModel.Notes;
            UpdatedDate = DateTime.Now;
            CreatedDate = DateTime.Now;
            CreatedUserId = 0;
            UpdatedUserId = 0;
            UpdatedSiteId = 0;
            AppointmentStatusId = 1;
            MarketingId = 0;
            ResourceId = 0;
        }

        public Appointment(AppointmentCreateModel createModel)
        {
            PatientId = createModel.PatientId;
            AppointmentTypeId = createModel.AppointmentTypeId ?? 0;
            ProviderId = createModel.ProviderId;
            SiteId = createModel.SiteId;
            StartsAt = createModel.StartsAt;
            EndsAt = createModel.EndsAt;
            Notes = createModel.Notes;
            AppointmentStatusId = createModel.AppointmentStatusId;
            AppointmentTypeId = createModel.AppointmentTypeId;
            AuthorizationId = createModel.AuthorizationId;
            BillToProviderId = createModel.BillToProviderId;
            Custom1 = createModel.Custom1;
            Custom2 = createModel.Custom2;
            MarketingId = createModel.MarketingReferenceId;
            NextContactDate = createModel.NextContact;
            Notes = createModel.Notes;
            NotificationStatus = (NotificationStatus)(createModel.NotificationStatus ?? 0);
            AddToCancellationList = createModel.AddToCancellationList;
            PatientId = createModel.PatientId;
            ProviderId = createModel.ProviderId;
            ResourceId = createModel.ResourceId;
            SiteId = createModel.SiteId;
            StartsAt = createModel.StartsAt;
            EndsAt = createModel.EndsAt;
            OtStatus = createModel.OpportunityStatus;
        }
       
        //public Guid Guid { get; set; }
        /// <summary>
        /// The id of the patient
        /// </summary>
        public int PatientId { get; set; }
        public int UpdatedSiteId { get; set; }
        public int? AppointmentTypeId { get; set; }
        public int ProviderId { get; set; }
        public int AppointmentStatusId { get; set; }
        public int SiteId { get; set; }
        public int SyncSiteId { get; set; }
        public int? ResourceId { get; set; }
        public int? MarketingId { get; set; }
        public int ReferringPhysicianId { get; set; }
        public int? RecurringIntervalId { get; set; }
        public int? RecurringParentId { get; set; }
        public int? RecurringItemNumber { get; set; }
        public int AuthorizationId { get; set; }
        public int BillToProviderId { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public DateTime? NextContactDate { get; set; }
        public NotificationStatus NotificationStatus { get; set; }
        public bool AddToCancellationList { get; set; }
        public OpportunityStatusEnum OtStatus { get; set; }
        public int OtStatusDescriptionId { get; set; }
        public OtStatusDescription OtStatusDescription { get; set; }
        public string Notes { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public bool Slp => AppointmentType != null && AppointmentType.Slp;
        public DateTime CreatedDate { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? UpdatedUserId { get; set; }

        // EF-mapped objects based on foreign key
        public AppointmentType AppointmentType { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
        public Patient Patient { get; set; }
        [TimsObject]
        public ApptRecurringInterval RecurringInterval { get; set; }
        public Provider Provider { get; set; }
        public ICollection<PosDocument> PosDocuments { get; set; }
        public string UpdatedByUserName { get; set; }
        public Site Site { get; set; }
        public string Opportunity => EnumUtilities.GetDescriptionFromEnum(OtStatus);
        public string OpportunityDescription { get; set; }
    }
}
