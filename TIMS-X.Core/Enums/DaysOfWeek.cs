using System;
using System.ComponentModel;

namespace TIMS_X.Core.Enums
{
    public enum DaysOfWeekEnum
    {
        [Description("Monday")] Monday = 1,
        [Description("Tuesday")] Tuesday = 2,
        [Description("Wednesday")] Wednesday = 3,
        [Description("Thursday")] Thursday = 4,
        [Description("Friday")] Friday = 5,
        [Description("Saturday")] Saturday = 6,
        [Description("Sunday")] Sunday = 7
    }

    public class DaysOfWeek
    {
        public static DaysOfWeekEnum FromDayOfWeek(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek == DayOfWeek.Sunday)
                return DaysOfWeekEnum.Sunday;

            return (DaysOfWeekEnum)(int)dayOfWeek;
        }
    }
}