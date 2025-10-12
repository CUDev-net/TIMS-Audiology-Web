using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain;

namespace TIMS_X.Core.Models
{
    public class ScheduleItem
    {
        public ScheduleItem() { }

        public ScheduleItem(Appointment appointment)
        {
            Id = appointment.Id;
            FromDate = appointment.StartsAt;
            ToDate = appointment.EndsAt;
            TypeId = appointment.AppointmentTypeId;
            Type = appointment.AppointmentType?.Name;
            StatusId = appointment.AppointmentStatusId;
            Status = appointment.AppointmentStatus?.Name;
            PatientId = appointment.PatientId;
            Patient = appointment.Patient?.LastFirstMiddle;
            SiteId = appointment.SiteId;
            Site = null;
            ProviderId = appointment.ProviderId;
            Provider = null;
            ResourceId = appointment.ResourceId;
            Resource = null;
            Color = appointment.AppointmentType?.Color;
        }
        public ScheduleItem(Appointment appointment, RecurringIntervalOccurrence occurrence)
        {
            Id = appointment.Id;
            FromDate = occurrence.StartsAt;
            ToDate = occurrence.EndsAt;
            TypeId = appointment.AppointmentTypeId;
            Type = appointment.AppointmentType?.Name;
            StatusId = appointment.AppointmentStatusId;
            Status = appointment.AppointmentStatus?.Name;
            PatientId = appointment.PatientId;
            Patient = appointment.Patient?.LastFirstMiddle;
            SiteId = appointment.SiteId;
            Site = null;
            ProviderId = appointment.ProviderId;
            Provider = null;
            ResourceId = appointment.ResourceId;
            Resource = null;
            Color = appointment.AppointmentType?.Color;

        }
        public ScheduleItem(Schedule schedule)
        {
            Id = schedule.Id;
            FromDate = schedule.StartsAt;
            ToDate = schedule.EndsAt;
            TypeId = null;
            PatientId = null;
            SiteId = schedule.SiteId;
            Site = null;
            ProviderId = schedule.ProviderId;
            Provider = null;
            Color = schedule.Color;
        }
        public ScheduleItem(Schedule schedule, RecurringIntervalOccurrence occurrence)
        {
            Id = schedule.Id;
            FromDate = occurrence.StartsAt;
            ToDate = occurrence.EndsAt;
            TypeId = null;
            PatientId = null;
            SiteId = schedule.SiteId;
            Site = null;
            ProviderId = schedule.ProviderId;
            Provider = null;
            Color = schedule.Color;
        }
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public int? TypeId { get; set; }
        public string Type { get; set; }
        public int? StatusId { get; set; }
        public string Status { get; set; }

        public int? PatientId { get; set; }
        public string Patient { get; set; }

        public int? SiteId { get; set; }
        public string Site { get; set; }

        public int? ProviderId { get; set; }
        public string Provider { get; set; }

        public int? ResourceId { get; set; }
        public string Resource { get; set; }
        public int? Color { get; set; }
        public string Subject
        {
            get
            {
                if (Patient != null)
                {
                    return Patient + Environment.NewLine + Type;
                }
                else
                {
                    return Type;
                }
            }
        }
    }
}
