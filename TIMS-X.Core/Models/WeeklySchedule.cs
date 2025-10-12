using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain;

namespace TIMS_X.Core.Models
{
    public class WeeklySchedule
    {
        public WeeklySchedule() { }

        public WeeklySchedule(IEnumerable<UserSiteHours> userSiteHours)
        {
            foreach (var siteHours in userSiteHours)
            {
                switch ((DayOfWeek)siteHours.DayNum)
                {
                    case DayOfWeek.Sunday:
                        SundayStart = siteHours.StartTime;
                        SundayEnd = siteHours.EndTime;
                        break;
                    case DayOfWeek.Monday:
                        MondayStart = siteHours.StartTime;
                        MondayEnd = siteHours.EndTime;
                        break;
                    case DayOfWeek.Tuesday:
                        TuesdayStart = siteHours.StartTime;
                        TuesdayEnd = siteHours.EndTime;
                        break;
                    case DayOfWeek.Wednesday:
                        WednesdayStart = siteHours.StartTime;
                        WednesdayEnd = siteHours.EndTime;
                        break;
                    case DayOfWeek.Thursday:
                        ThursdayStart = siteHours.StartTime;
                        ThursdayEnd = siteHours.EndTime;
                        break;
                    case DayOfWeek.Friday:
                        FridayStart = siteHours.StartTime;
                        FridayEnd = siteHours.EndTime;
                        break;
                    case DayOfWeek.Saturday:
                        SaturdayStart = siteHours.StartTime;
                        SaturdayEnd = siteHours.EndTime;
                        break;
                }
            }
        }
        public WeeklySchedule(IEnumerable<ScheduleTimeSlot> timeSlots)
        {
            foreach (var timeSlot in timeSlots)
            {
                switch ((DayOfWeek)timeSlot.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        SundayStart = timeSlot.StartTime;
                        SundayEnd = timeSlot.EndTime;
                        break;
                    case DayOfWeek.Monday:
                        MondayStart = timeSlot.StartTime;
                        MondayEnd = timeSlot.EndTime;
                        break;
                    case DayOfWeek.Tuesday:
                        TuesdayStart = timeSlot.StartTime;
                        TuesdayEnd = timeSlot.EndTime;
                        break;
                    case DayOfWeek.Wednesday:
                        WednesdayStart = timeSlot.StartTime;
                        WednesdayEnd = timeSlot.EndTime;
                        break;
                    case DayOfWeek.Thursday:
                        ThursdayStart = timeSlot.StartTime;
                        ThursdayEnd = timeSlot.EndTime;
                        break;
                    case DayOfWeek.Friday:
                        FridayStart = timeSlot.StartTime;
                        FridayEnd = timeSlot.EndTime;
                        break;
                    case DayOfWeek.Saturday:
                        SaturdayStart = timeSlot.StartTime;
                        SaturdayEnd = timeSlot.EndTime;
                        break;
                }
            }
        }

        public DateTime? SundayStart { get; set; }
        public DateTime? SundayEnd { get; set; }
        public DateTime? MondayStart { get; set; }
        public DateTime? MondayEnd { get; set; }
        public DateTime? TuesdayStart { get; set; }
        public DateTime? TuesdayEnd { get; set; }
        public DateTime? WednesdayStart { get; set; }
        public DateTime? WednesdayEnd { get; set; }
        public DateTime? ThursdayStart { get; set; }
        public DateTime? ThursdayEnd { get; set; }
        public DateTime? FridayStart { get; set; }
        public DateTime? FridayEnd { get; set; }
        public DateTime? SaturdayStart { get; set; }
        public DateTime? SaturdayEnd { get; set; }
    }
}
