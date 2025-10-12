using System;
using System.Collections.ObjectModel;

namespace TIMS_X.Core.Domain
{
    public class CalendarSummaryItem
    {
        public string ItemId { get; set; }
        public DateTime ItemStart { get; set; }
        public DateTime ItemEnd { get; set; }
        public bool? AddToCancellationList { get; set; }
        public int PatientId { get; set; }
        public string PatientLastname { get; set; }
        public string PatientInitial { get; set; }
        public string PatientFirstName { get; set; }
        public int? PatientSiteId { get; set; }
        public int? ProviderId { get; set; }
        public string ProviderFirstName { get; set; }
        public string ProviderLastName { get; set; }
        public int? ProviderColorValue { get; set; }
        public int? StatusId { get; set; }
        public string StatusName { get; set; }
        public int? StatusShow { get; set; }
        public int? StatusIconId { get; set; }
        public int? SiteId { get; set; }
        public string SiteName { get; set; }
        public int? SiteColorValue { get; set; }
        public int? ResourceId { get; set; }
        public string ResourceName { get; set; }
        public int? ResourceColorValue { get; set; }
        public int? AppointmentTypeId { get; set; }
        public string AppointmentTypeName { get; set; }
        public int? AppointmentTypeColorValue { get; set; }
        public bool? AppointmentTypeIsSLP { get; set; }
        public string Notes { get; set; }
        public string Location { get; set; }
        public Guid? CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int? ScheduleColorValue { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? IsRecurring { get; set; }
        public int RecurranceID { get; set; }
        public int? IntervalType { get; set; }
        public int? IntervalSubType { get; set; }
        public int? DayInterval { get; set; }
        public int? WeekInterval { get; set; }
        public bool? IsMondaySet { get; set; }
        public bool? IsTuesdaySet { get; set; }
        public bool? IsWednesdaySet { get; set; }
        public bool? IsThursdaySet { get; set; }
        public bool? IsFridaySet { get; set; }
        public bool? IsSaturdaySet { get; set; }
        public bool? IsSundaySet { get; set; }
        public int? DayOfMonth { get; set; }
        public int? MonthInterval { get; set; }
        public int? DayOfWeek { get; set; }
        public int? DayQualifier { get; set; }
        public int? Month { get; set; }
        public int? EndType { get; set; }
        public int? EndAfter { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string AppointmentTypeWebColor { get; set; }
        public string SiteWebColor { get; set; }
        public string ProviderWebColor { get; set; }
        public ObservableCollection<RecurringIntervalRemoved> RemovedInstances { get; set; }
    }
}
