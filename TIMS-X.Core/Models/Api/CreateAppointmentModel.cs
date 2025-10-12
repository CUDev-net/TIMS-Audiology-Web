using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TIMS_X.Core.Models.Api
{
    public class CreateAppointmentModel
    {
        /// <summary>
        /// The id of the patient
        /// </summary>
        public int PatientId { get; set; }
        /// <summary>
        /// The id of the appointment type
        /// </summary>
        public int AppointmentTypeId { get; set; }
        /// <summary>
        /// The id of the provider
        /// </summary>
        public int ProviderId { get; set; }

        /// <summary>
        /// The id of the site
        /// </summary>
        public int SiteId { get; set; }
        /// <summary>
        /// The starting time of the appointment
        /// </summary>
        public DateTime StartsAt { get; set; }
        /// <summary>
        /// The ending time of the appointment
        /// </summary>
        public DateTime EndsAt { get; set; }
        /// <summary>
        /// Appointment notes
        /// </summary>
        public string Notes { get; set; }
    }
}
