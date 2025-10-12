using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Models
{
    public class PatientNotification : IPatientNotification
    {
        // Culture-specific datetime formatter
        public DateTimeFormatInfo DateTimeFormat { get; set; }

        public string AppointmentDatePlaceholder
        {
            get
            {
                return AppointmentStartsAt.ToString(DateTimeFormat.ShortDatePattern);
                //return AppointmentStartsAt.ToShortDateString();
            }
        }

        public string AppointmentDayOfWeekPlaceholder
        {
            get
            {
                return DateTimeFormat.GetDayName(AppointmentStartsAt.DayOfWeek);
                //return AppointmentStartsAt.ToString("dddd");
            }
        }

        public DateTime AppointmentEndsAt
        {
            get;
            set;
        }

        public string AppointmentEndTimePlaceholder
        {
            get
            {
                return AppointmentEndsAt.ToString(DateTimeFormat.ShortTimePattern);
                //return AppointmentEndsAt.ToShortTimeString();
            }
        }

        public int AppointmentId
        {
            get;
            set;
        }

        public string AppointmentSite
        {
            get;
            set;
        }

        public string AppointmentSitePlaceholder
        {
            get
            {
                return AppointmentSite;
            }
        }

        public string AppointmentSiteAddress1 { get; set; }
        public string AppointmentSiteAddress1Placeholder => AppointmentSiteAddress1;

        public string AppointmentSiteAddress2 { get; set; }
        public string AppointmentSiteAddress2Placeholder => AppointmentSiteAddress2;

        public string AppointmentSiteCityStateZip { get; set; }
        public string AppointmentSiteCityStateZipPlaceholder => AppointmentSiteCityStateZip;

        public string AppointmentSitePhone { get; set; }
        public string AppointmentSitePhonePlaceholder => AppointmentSitePhone;


        public DateTime AppointmentStartsAt
        {
            get;
            set;
        }

        public string AppointmentStartTimePlaceholder
        {
            get
            {
                return AppointmentStartsAt.ToString(DateTimeFormat.ShortTimePattern);
                //return AppointmentStartsAt.ToShortTimeString();
            }
        }

        public string AppointmentType
        {
            get;
            set;
        }

        public string AppointmentTypePlaceholder
        {
            get
            {
                return AppointmentType;
            }
        }

        public LanguageEnum Language
        {
            get;
            set;
        }

        public string ConfirmationLink
        {
            get;
            set;
        }

        public string ConfirmationLinkPlaceholder
        {
            get
            {
                return ConfirmationLink;
            }
        }

        public string CallForRescheduleLink
        {
            get;
            set;
        }

        public string CallForRescheduleLinkPlaceholder
        {
            get
            {
                return CallForRescheduleLink;
            }
        }
        public string CancelLink
        {
            get;
            set;
        }

        public string CancelLinkPlaceholder
        {
            get
            {
                return CancelLink;
            }
        }

        public string DigitalPatientIntakeLink
        {
            get;
            set;
        }

        public string DigitalPatientIntakeLinkPlaceholder
        {
            get
            {
                return DigitalPatientIntakeLink;
            }
        }

        public string PatientFirstName
        {
            get;
            set;
        }

        public string PatientFirstNamePlaceholder
        {
            get
            {
                return PatientFirstName;
            }
        }

        public int PatientId
        {
            get;
            set;
        }

        public string PatientLastName
        {
            get;
            set;
        }

        public string PatientNamePlaceholder
        {
            get
            {
                return $"{PatientFirstName} {PatientLastName}";
            }
        }

        public string ProviderFirstName
        {
            get;
            set;
        }

        public string ProviderLastName
        {
            get;
            set;
        }

        public string ProviderNamePlaceholder
        {
            get
            {
                return $"{ProviderFirstName} {ProviderLastName}";
            }
        }


    }
}
