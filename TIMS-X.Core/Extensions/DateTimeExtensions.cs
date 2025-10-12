using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToEndOfDay(this DateTime date)
        {
            return date.Date.AddDays(1).AddMilliseconds(-1);
        }

        public static DateTime SetTime(this DateTime date, DateTime? time)
        {
            var timeValue = time.GetValueOrDefault();
            return new DateTime(date.Year, date.Month, date.Day,
                timeValue.Hour, timeValue.Minute, timeValue.Second);
        }
    }
}
