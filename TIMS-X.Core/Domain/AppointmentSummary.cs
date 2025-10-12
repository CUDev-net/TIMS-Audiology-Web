using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class AppointmentSummary : Entity
    {
        public int PatientId { get; set; }
        public string AppointmentType { get; set; }
        public string AppointmentStatus { get; set; }
        public string Site { get; set; }
        public string ProviderFirstName { get; set; }
        public string ProviderLastName { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
