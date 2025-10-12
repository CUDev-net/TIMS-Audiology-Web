using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.Dtos
{
    public class AppointmentDto
    {
        public AppointmentDto(Appointment appointment)
        {
            Appointment = appointment;
        }

        public Appointment Appointment { get; }
        public string Appointment_Type_Color { get; set; }
        public string Provider_Color { get; set; }
        public string ProviderName { get; set; }
        public string Site_Color { get; set; }
        public string SiteName { get; set; }
        public string TypeName { get; set; }
        public string PatientName { get; set; }
    }
}