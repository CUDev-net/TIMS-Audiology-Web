using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Models
{
    public interface IPatientNotification
    {
        int PatientId { get; set; }
        int AppointmentId { get; set; }
    }
}
