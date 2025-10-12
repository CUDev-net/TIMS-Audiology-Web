using System;

namespace Tims.Dal.Models
{
    public class AppointmentItemSummary
    {
#pragma warning disable CS8618
        public string id { get; set; }
        public bool status_show { get; set; }
        public int? appointment_type_id { get; set; }
        public string appointment_type_web_color { get; set; }
        public int? patient_site_id { get; set; }
        public int? provider_id { get; set; }
        public int? provider_color_value { get; set; }
        public string provider_web_color { get; set; }
        public int? status_id { get; set; }
        public string status_name { get; set; }
        public string notes { get; set; }
        public int patient_id { get; set; }
        public int? resource_id { get; set; }
        public string resource_name { get; set; }
        public int? resource_color_value { get; set; }
        public string patient_name { get; set; }
        public string provider_name { get; set; }
        public string appointment_type_name { get; set; }
        public int? appointment_type_color_value { get; set; }
        public int? site_id { get; set; }
        public int? site_color_value { get; set; }
        public string site_web_color { get; set; }
        public string site_name { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }

#pragma warning restore CS8618
    }
}