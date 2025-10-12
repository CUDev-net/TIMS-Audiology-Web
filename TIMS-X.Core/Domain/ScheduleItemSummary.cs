using System;

namespace TIMS_X.Core.Domain
{
    public interface IScheduleItemSummary
    {
        int? provider_color_value { get; set; }
        string provider_web_color { get; set; }
        int? color { get; set; }
        string color_web { get; set; }
        string site_web_color { get; set; }
        int? site_color_value { get; set; }
    }

    public class ScheduleItemSummary : IScheduleItemSummary
    {
#pragma warning disable CS8618
        public string id { get; set; }
        public string location { get; set; }
        public string notes { get; set; }
        public int? provider_id { get; set; }
        public int? provider_color_value { get; set; }
        public string provider_name { get; set; }
        public string provider_web_color { get; set; }
        public int? site_id { get; set; }
        public int? site_color_value { get; set; }
        public int? color { get; set; }
        public string color_web { get; set; }
        public string site_web_color { get; set; }
        public string site_name { get; set; }
        public string title { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
#pragma warning restore CS8618
    }
}