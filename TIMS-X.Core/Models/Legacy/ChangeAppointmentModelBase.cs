using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Models.Legacy
{
    public class ChangeAppointmentModelBase
    {
        #region ChangeAppointmentModelBase Members

        public int Id 
        { 
            get; 
            set; 
        }

        public int AppointmentStatusId
        {
            get;
            set;
        }

        public int? AppointmentTypeId
        {
            get;
            set;
        }

        public int AuthorizationId
        {
            get;
            set;
        }

        public int BillToProviderId
        {
            get;
            set;
        }

        public string Custom1
        {
            get;
            set;
        }

        public string Custom2
        {
            get;
            set;
        }
        

        public bool IsRecurring
        {
            get;
            set;
        }

        public int? MarketingReferenceId
        {
            get;
            set;
        }

        public DateTime? NextContact
        {
            get;
            set;
        }

        public string Notes
        {
            get;
            set;
        }

        public int? NotificationStatus
        {
            get;
            set;
        }

        public bool AddToCancellationList
        {
            get;
            set;
        }

        public OpportunityStatusEnum OpportunityStatus
        {
            get;
            set;
        }

        public int PatientId
        {
            get;
            set;
        }

        public int ProviderId
        {
            get;
            set;
        }

        public int? ResourceId
        {
            get;
            set;
        }

        public int SiteId
        {
            get;
            set;
        }

        public DateTime StartsAt
        {
            get;
            set;
        }
        public DateTime EndsAt
        {
            get;
            set;
        }

        #endregion ChangeAppointmentModelBase Members

    }
}
