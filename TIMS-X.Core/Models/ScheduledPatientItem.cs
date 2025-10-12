using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Models
{
    public class ScheduledPatientItem : PatientItem
    {
        public string AppointmentTime { get; set; }
        public string AppointmentDuration { get; set; }
        public string ProviderName { get; set; }
        public string AppointmentStatus { get; set; }
        public string AppointmentType { get; set; }
        public string NameAndAppointmentTime => $"{LastName}, {FirstName} {(string.IsNullOrEmpty(Initial) ? string.Empty : ", " + Initial + ".")} - {AppointmentTime}";
    }
}
