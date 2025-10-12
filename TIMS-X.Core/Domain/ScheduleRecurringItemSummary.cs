using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TIMS_X.Core.Domain
{
    public class ScheduleRecurringItemSummary : IScheduleItemSummary
    {
        public ScheduleRecurringItemSummary()
        {
            RemovedInstances = new ObservableCollection<RecurringIntervalRemoved>();
            DeletedOccurrences = new List<int>();
        }
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
        // Recurrence data
        public int recurring_interval_id {get; set; }
        public int interval_type { get; set; }
        public int interval_sub_type { get; set; }
        public int day_interval { get; set; }
        public int week_interval { get; set; }
        public bool is_monday_set { get; set; }
        public bool is_tuesday_set { get; set; }
        public bool is_wednesday_set { get; set; }
        public bool is_thursday_set { get; set; }
        public bool is_friday_set { get; set; }
        public bool is_saturday_set { get; set; }
        public bool is_sunday_set { get; set; }
        public int day_of_month { get; set; }
        public int month_interval { get; set; }
        public int day_of_week { get; set; }
        public int day_qualifier { get; set; }
        public int month { get; set; }
        public int end_type { get; set; }
        public int end_after { get; set; }
        public DateTime recurrence_start_date { get; set; }
        public DateTime recurrence_end_date { get; set; }
        // Client data
        public string rec_type { get; set; }
        public string event_length { get; set; }
        public string event_pid { get; set; }
        public List<int> DeletedOccurrences { get; set; }
        public ObservableCollection<RecurringIntervalRemoved> RemovedInstances { get; set; }

#pragma warning restore CS8618
    }
}