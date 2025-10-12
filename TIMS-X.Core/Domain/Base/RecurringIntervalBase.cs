using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Extensions;
using TIMS_X.Core.Models;

namespace TIMS_X.Core.Domain.Base
{
    public class RecurringIntervalBase : AuditableEntity, ICreateDateAudited, IUpdateDateAudited
    {
        public int IntervalType { get; set; }
        public int SubType { get; set; }
        public int DayInterval { get; set; }
        public int WeekInterval { get; set; }
        public bool IsMondaySet { get; set; }
        public bool IsTuesdaySet { get; set; }
        public bool IsWednesdaySet { get; set; }
        public bool IsThursdaySet { get; set; }
        public bool IsFridaySet { get; set; }
        public bool IsSaturdaySet { get; set; }
        public bool IsSundaySet { get; set; }
        public int DayOfMonth { get; set; }
        public int MonthInterval { get; set; }
        public int DayOfWeek { get; set; }
        public int DayQualifier { get; set; }
        public int Month { get; set; }
        public int EndType { get; set; }
        public int EndOccurs { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        private int _CalculateCurrentDayOfWeek(DateTime dt)
        {
            return dt.DayOfWeek == System.DayOfWeek.Sunday ? 7 : (int)dt.DayOfWeek;
        }

        private DateTime _CalculateDateFromQualifierAndDayOfWeek(int dayQualifier, int day, DateTime date)
        {
            var result = date.AddDays(-date.Day); // set the date on the first day

            if (dayQualifier != (int)DayQualifierEnum.last)
            {
                var qualifier = 0;
                while (dayQualifier != qualifier)
                {
                    result = result.AddDays(1);

                    while (_CalculateCurrentDayOfWeek(result) != day) result = result.AddDays(1);

                    qualifier++;
                }
            }
            else
            {
                // Look for the first Monday/Tuesday/...
                result = result.AddDays(1);

                while (_CalculateCurrentDayOfWeek(result) != day) result = result.AddDays(1);

                while (result.Month == date.Month) result = result.AddDays(7);

                result = result.AddDays(-7);
            }

            return result;
        }

        private static void _CalculateDateRangeForDailyIntervals(IRecurrable recurrableItem,
            RecurringIntervalBase interval)
        {
            var startDate = recurrableItem.StartsAt.Date;
            var endDate = recurrableItem.EndsAt.Date;

            var intervalEndType = (RecurrenceEndTypeEnum)interval.EndType;
            if (intervalEndType != RecurrenceEndTypeEnum.EndByDate) interval.EndDate = recurrableItem.EndsAt;

            switch ((DailyIntervalSubTypeEnum)interval.SubType)
            {
                case DailyIntervalSubTypeEnum.NthDay:

                    // END DATE: Calculation based on user's settings (occurence, day interval)
                    switch (intervalEndType)
                    {
                        case RecurrenceEndTypeEnum.EndAfterOccurrences:
                            endDate = startDate.Date.AddDays(interval.DayInterval * (interval.EndOccurs - 1));
                            break;

                        case RecurrenceEndTypeEnum.EndByDate:
                            while (endDate.Date <= interval.EndDate) endDate = endDate.AddDays(interval.DayInterval);
                            endDate = endDate.AddDays(-interval.DayInterval);
                            break;
                    }

                    break;

                case DailyIntervalSubTypeEnum.Weekday:

                    if (intervalEndType != RecurrenceEndTypeEnum.NotSet)
                        // Calculation of Begin Date (if adjustment is necessary)
                        while (startDate.DayOfWeek == System.DayOfWeek.Saturday
                               || startDate.DayOfWeek == System.DayOfWeek.Sunday)
                        {
                            startDate = startDate.AddDays(1);
                            endDate = endDate.AddDays(1);
                        }

                    switch (intervalEndType)
                    {
                        case RecurrenceEndTypeEnum.EndAfterOccurrences:
                            var occurences = 1;
                            while (occurences < interval.EndOccurs)
                            {
                                switch (endDate.DayOfWeek)
                                {
                                    case System.DayOfWeek.Friday:
                                        endDate = endDate.AddDays(3);
                                        break;
                                    case System.DayOfWeek.Saturday:
                                        endDate = endDate.AddDays(2);
                                        break;
                                    default:
                                        endDate = endDate.AddDays(1);
                                        break;
                                }

                                occurences++;
                            }

                            break;

                        case RecurrenceEndTypeEnum.EndByDate:
                            endDate = interval.EndDate;
                            switch (endDate.DayOfWeek)
                            {
                                case System.DayOfWeek.Saturday:
                                    endDate = endDate.AddDays(-1);
                                    break;
                                case System.DayOfWeek.Sunday:
                                    endDate = endDate.AddDays(-2);
                                    break;
                            }

                            break;
                    }

                    break;

                case DailyIntervalSubTypeEnum.MonWedFri:

                    if (intervalEndType != RecurrenceEndTypeEnum.NotSet)
                        // Calculation of Begin Date (if adjustment is necessary)
                        while (startDate.DayOfWeek != System.DayOfWeek.Monday
                               && startDate.DayOfWeek != System.DayOfWeek.Wednesday
                               && startDate.DayOfWeek != System.DayOfWeek.Friday)
                        {
                            startDate = startDate.AddDays(1);
                            endDate = endDate.AddDays(1);
                        }

                    switch (intervalEndType)
                    {
                        case RecurrenceEndTypeEnum.EndAfterOccurrences:
                            var occurrences = 1;
                            while (occurrences < interval.EndOccurs)
                            {
                                switch (endDate.DayOfWeek)
                                {
                                    case System.DayOfWeek.Friday:
                                        endDate = endDate.AddDays(3);
                                        break;
                                    case System.DayOfWeek.Sunday:
                                    case System.DayOfWeek.Thursday:
                                    case System.DayOfWeek.Tuesday:
                                        endDate = endDate.AddDays(1);
                                        break;
                                    default:
                                        endDate = endDate.AddDays(2);
                                        break;
                                }

                                occurrences++;
                            }

                            break;

                        case RecurrenceEndTypeEnum.EndByDate:
                            endDate = interval.EndDate;
                            switch (endDate.DayOfWeek)
                            {
                                case System.DayOfWeek.Saturday:
                                case System.DayOfWeek.Thursday:
                                case System.DayOfWeek.Tuesday:
                                    endDate = endDate.AddDays(-1);
                                    break;
                                case System.DayOfWeek.Sunday:
                                    endDate = endDate.AddDays(-2);
                                    break;
                            }

                            break;
                    }

                    break;

                case DailyIntervalSubTypeEnum.TuesThurs:

                    if (intervalEndType != RecurrenceEndTypeEnum.NotSet)
                        // Calculation of Begin Date (if adjustment is necessary)
                        while (startDate.DayOfWeek != System.DayOfWeek.Tuesday
                               && startDate.DayOfWeek != System.DayOfWeek.Thursday)
                        {
                            startDate = startDate.AddDays(1);
                            endDate = endDate.AddDays(1);
                        }

                    switch (intervalEndType)
                    {
                        case RecurrenceEndTypeEnum.EndAfterOccurrences:
                            var occurrences = 1;
                            while (occurrences < interval.EndOccurs)
                            {
                                switch (endDate.DayOfWeek)
                                {
                                    case System.DayOfWeek.Thursday:
                                        endDate = endDate.AddDays(5);
                                        break;
                                    case System.DayOfWeek.Friday:
                                        endDate = endDate.AddDays(4);
                                        break;
                                    case System.DayOfWeek.Saturday:
                                        endDate = endDate.AddDays(3);
                                        break;
                                    case System.DayOfWeek.Sunday:
                                    case System.DayOfWeek.Tuesday:
                                        endDate = endDate.AddDays(2);
                                        break;
                                    case System.DayOfWeek.Wednesday:
                                    case System.DayOfWeek.Monday:
                                        endDate = endDate.AddDays(1);
                                        break;
                                }

                                occurrences++;
                            }

                            break;

                        case RecurrenceEndTypeEnum.EndByDate:
                            endDate = interval.EndDate;
                            switch (endDate.DayOfWeek)
                            {
                                case System.DayOfWeek.Wednesday:
                                case System.DayOfWeek.Friday:
                                    endDate = endDate.AddDays(-1);
                                    break;
                                case System.DayOfWeek.Monday:
                                    endDate = endDate.AddDays(-4);
                                    break;
                                case System.DayOfWeek.Sunday:
                                    endDate = endDate.AddDays(-3);
                                    break;
                                case System.DayOfWeek.Saturday:
                                    endDate = endDate.AddDays(-2);
                                    break;
                            }

                            break;
                    }

                    break;
            }

            // Add the time of day to the date component of the start and end date.
            if (interval.StartDate == DateTime.MinValue)
                interval.StartDate = startDate.Date + recurrableItem.StartsAt.TimeOfDay;
            //do not update EndDate if previously set to a date later than the current item.  Non-Real appts disappear from calendar after update to previous "real" appt.
            //Bug found while working on Mantis 3860.  Preserving EndDate does not seem to prevent this.  Bug still occurs.
            if (interval.EndDate < endDate)
                interval.EndDate = endDate.Date + recurrableItem.EndsAt.TimeOfDay;
        }

        private void _CalculateDateRangeForMonthlyIntervals(IRecurrable recurrableItem, RecurringIntervalBase interval)
        {
            interval.StartDate = recurrableItem.StartsAt.Date;

            var intervalEndType = (RecurrenceEndTypeEnum)interval.EndType;
            if (intervalEndType != RecurrenceEndTypeEnum.EndByDate)
                interval.EndDate = recurrableItem.EndsAt;

            var intervalEndTypeIsSet = intervalEndType == RecurrenceEndTypeEnum.NoEndDate
                                       || intervalEndType == RecurrenceEndTypeEnum.EndAfterOccurrences
                                       || intervalEndType == RecurrenceEndTypeEnum.EndByDate;

            if (interval.SubType == (int)MonthlyIntervalSubTypeEnum.DayOfMonth)
            {
                // Helper field
                var lastDay = false; // is last day of month?

                // check if DayNum exists in current month
                var maxDayNum = DateTime.DaysInMonth(interval.StartDate.Year, interval.StartDate.Month);
                if (interval.DayOfMonth > maxDayNum) lastDay = true;

                // BEGIN DATE: Calculation (if adjustment is necessary)
                if (intervalEndTypeIsSet)
                {
                    var startDate = interval.StartDate.Date;

                    // Calculation of Begin Date(adjusted) and End Date based on user's settings
                    var startDay = startDate.Day;
                    var dayOfMonth = interval.DayOfMonth;

                    if (dayOfMonth > startDay) // stay in the same selected month
                    {
                        startDate = startDate.AddDays((lastDay ? maxDayNum : dayOfMonth) - startDay);
                    }
                    else if (dayOfMonth < startDay) // start with next month
                    {
                        startDate = startDate.AddMonths(1);
                        startDate = startDate.AddDays(dayOfMonth -
                                                      startDay); // Substract the days off that are too much
                    }

                    interval.StartDate = startDate;
                }

                // END DATE: Calculation based on user's settings (occurence, day interval)
                if (intervalEndType == RecurrenceEndTypeEnum.NoEndDate)
                {
                    interval.EndDate = interval.StartDate.Date;
                }
                else if (intervalEndType == RecurrenceEndTypeEnum.EndAfterOccurrences)
                {
                    var endDate = interval.StartDate.Date;

                    endDate = endDate.AddMonths(interval.MonthInterval * (interval.EndOccurs - 1));

                    if (lastDay)
                        endDate = endDate.AddDays(DateTime.DaysInMonth(endDate.Year, endDate.Month) - endDate.Day);

                    interval.EndDate = endDate;
                }
                else if (intervalEndType == RecurrenceEndTypeEnum.EndByDate)
                {
                    var endDate = interval.StartDate.Date;

                    while (endDate.Date <= interval.EndDate)
                    {
                        endDate = endDate.AddMonths(interval.MonthInterval);

                        if (lastDay)
                            endDate = endDate.AddDays(DateTime.DaysInMonth(endDate.Year, endDate.Month) - endDate.Day);
                    }

                    endDate = endDate.AddMonths(-interval.MonthInterval);

                    if (lastDay)
                        endDate = endDate.AddDays(DateTime.DaysInMonth(endDate.Year, endDate.Month) - endDate.Day);

                    interval.EndDate = endDate;
                }
            }
            else if (interval.SubType == (int)MonthlyIntervalSubTypeEnum.DayOfWeek)
            {
                var dayQualifier = interval.DayQualifier;
                var dayOfWeek = interval.DayOfWeek;

                // BEGIN DATE: Calculation (if adjustment is necessary)
                if (intervalEndTypeIsSet)
                {
                    var startDate = interval.StartDate.Date;

                    // Calculate the correct begin date
                    var startDay = startDate.Day;

                    // Calculate date based on dayqualifier, daytype and month/year (based on BeginDate)
                    var adjustedDay =
                        _CalculateDateFromQualifierAndDayOfWeek(dayQualifier, dayOfWeek, interval.StartDate).Day;

                    if (adjustedDay > startDay) // stay in the same selected month
                    {
                        startDate = _CalculateDateFromQualifierAndDayOfWeek(dayQualifier, dayOfWeek, startDate);
                    }
                    else if (adjustedDay < startDay) // start with next month
                    {
                        startDate = startDate.AddMonths(1);
                        startDate = _CalculateDateFromQualifierAndDayOfWeek(dayQualifier, dayOfWeek, startDate);
                    }

                    interval.StartDate = startDate;
                }

                // END DATE: Calculation based on user's settings (occurence, day interval)
                if (intervalEndType == RecurrenceEndTypeEnum.NoEndDate)
                {
                    interval.EndDate = interval.StartDate.Date;
                }
                else if (intervalEndType == RecurrenceEndTypeEnum.EndAfterOccurrences)
                {
                    var endDate = interval.StartDate.Date;

                    endDate = endDate.AddMonths(interval.MonthInterval *
                                                (interval.EndOccurs - 1)); // only needed to get the next month
                    endDate = _CalculateDateFromQualifierAndDayOfWeek(dayQualifier, dayOfWeek,
                        endDate); // calculates date based on month (dt) and qualifier, daytype
                    interval.EndDate = endDate; // set calculated end date
                }
                else if (intervalEndType == RecurrenceEndTypeEnum.EndByDate)
                {
                    var endDate = interval.StartDate;

                    while (endDate.Date <= interval.EndDate.Date)
                    {
                        endDate = endDate.AddMonths(interval.MonthInterval); // only needed to get the next month
                        endDate = _CalculateDateFromQualifierAndDayOfWeek(dayQualifier, dayOfWeek,
                            endDate); // calculates date based on month (dt) and qualifier, daytype
                    }

                    endDate = endDate.AddMonths(-interval.MonthInterval);
                    endDate = _CalculateDateFromQualifierAndDayOfWeek(dayQualifier, dayOfWeek, endDate);

                    interval.EndDate = endDate;
                }
            }

            // Adjust start and end date to appropriate times:
            interval.StartDate = interval.StartDate.Date + recurrableItem.StartsAt.TimeOfDay;
            interval.EndDate = interval.EndDate.Date + recurrableItem.EndsAt.TimeOfDay;
        }

        private void _CalculateDateRangeForWeeklyIntervals(IRecurrable recurrableItem, RecurringIntervalBase interval)
        {
            interval.StartDate = recurrableItem.StartsAt.Date;

            var intervalEndType = (RecurrenceEndTypeEnum)interval.EndType;
            if (intervalEndType != RecurrenceEndTypeEnum.EndByDate)
                interval.EndDate = recurrableItem.EndsAt;

            var intervalEndTypeIsSet = intervalEndType == RecurrenceEndTypeEnum.NoEndDate
                                       || intervalEndType == RecurrenceEndTypeEnum.EndAfterOccurrences
                                       || intervalEndType == RecurrenceEndTypeEnum.EndByDate;

            // Get a list of days that are not-selected by the user
            var listNonSelected = new List<DayOfWeek>();
            if (!interval.IsMondaySet)
                listNonSelected.Add(System.DayOfWeek.Monday);
            if (!interval.IsTuesdaySet)
                listNonSelected.Add(System.DayOfWeek.Tuesday);
            if (!interval.IsWednesdaySet)
                listNonSelected.Add(System.DayOfWeek.Wednesday);
            if (!interval.IsThursdaySet)
                listNonSelected.Add(System.DayOfWeek.Thursday);
            if (!interval.IsFridaySet)
                listNonSelected.Add(System.DayOfWeek.Friday);
            if (!interval.IsSaturdaySet)
                listNonSelected.Add(System.DayOfWeek.Saturday);
            if (!interval.IsSundaySet)
                listNonSelected.Add(System.DayOfWeek.Sunday);

            // Get a list of days that are selected by the user
            var listSelected = new List<DayOfWeek>();
            if (interval.IsMondaySet)
                listSelected.Add(System.DayOfWeek.Monday);
            if (interval.IsTuesdaySet)
                listSelected.Add(System.DayOfWeek.Tuesday);
            if (interval.IsWednesdaySet)
                listSelected.Add(System.DayOfWeek.Wednesday);
            if (interval.IsThursdaySet)
                listSelected.Add(System.DayOfWeek.Thursday);
            if (interval.IsFridaySet)
                listSelected.Add(System.DayOfWeek.Friday);
            if (interval.IsSaturdaySet)
                listSelected.Add(System.DayOfWeek.Saturday);
            if (interval.IsSundaySet)
                listSelected.Add(System.DayOfWeek.Sunday);

            /*
			 * Helper Array (2-dim)
			 * -----------------------------------
			 * 
			 * e.g.:
			 *           isSelected  distanceToNext
			 * Monday   |     1     |       0
			 * Tuesday  |     1     |       0
			 * Wednesday|           |       2 (in order to get from Wednesday to Friday, it takes 2 steps)
			 * Thursday |           |       1 (in order to get from Thursday to Friday, it takes 1 step)
			 * Friday   |     1     |       0
			 * Saturday |           |       2 (in order to get from Saturday to Monday, it takes 2 steps)
			 * Sunday   |           |       1 (in order to get from Sunday to Monday, it takes 1 step)
			 * 
			 */
            var myArray = new int[7, 2]; // 7 rows, 2 columns

            // set first column of if the day is selected
            myArray[0, 0] = interval.IsMondaySet ? 1 : 0;
            myArray[1, 0] = interval.IsTuesdaySet ? 1 : 0;
            myArray[2, 0] = interval.IsWednesdaySet ? 1 : 0;
            myArray[3, 0] = interval.IsThursdaySet ? 1 : 0;
            myArray[4, 0] = interval.IsFridaySet ? 1 : 0;
            myArray[5, 0] = interval.IsSaturdaySet ? 1 : 0;
            myArray[6, 0] = interval.IsSundaySet ? 1 : 0;

            // set the second column with the distance from an unselected day to the next selected day (calculation)
            for (var i = 0; i < 7; i++)
                if (myArray[i, 0] == 1)
                    myArray[i, 1] = 0;
                else
                    for (var j = 0; j < 7; j++)
                        if (myArray[(i + j) % 7, 0] == 0)
                            myArray[i, 1] = myArray[i, 1] + 1;
                        else
                            break;


            // BEGIN DATE: Calculation (if adjustment is necessary)
            if (intervalEndTypeIsSet)
            {
                var startDate = interval.StartDate.Date;

                // Calculation of Begin Date (adjustment)
                while (listNonSelected.Contains(startDate.DayOfWeek))
                    startDate = startDate.AddDays(1);

                interval.StartDate = startDate;
            }

            // END DATE: Calculation based on user's settings (occurence, day interval)
            if (intervalEndType == RecurrenceEndTypeEnum.NoEndDate)
            {
                interval.EndDate = interval.StartDate.Date;
            }
            else if (intervalEndType == RecurrenceEndTypeEnum.EndAfterOccurrences)
            {
                var endDate = interval.StartDate.Date;

                // Calculation of End Date based on user's settings
                var occurrences = 1;
                while (occurrences < interval.EndOccurs)
                {
                    endDate = endDate.AddDays(1);
                    switch (endDate.DayOfWeek)
                    {
                        case System.DayOfWeek.Monday:
                            endDate = endDate.AddDays(myArray[0,
                                1]); // add so many days that it needs to get to the next selected day.
                            break;
                        case System.DayOfWeek.Tuesday:
                            endDate = endDate.AddDays(myArray[1, 1]);
                            break;
                        case System.DayOfWeek.Wednesday:
                            endDate = endDate.AddDays(myArray[2, 1]);
                            break;
                        case System.DayOfWeek.Thursday:
                            endDate = endDate.AddDays(myArray[3, 1]);
                            break;
                        case System.DayOfWeek.Friday:
                            endDate = endDate.AddDays(myArray[4, 1]);
                            break;
                        case System.DayOfWeek.Saturday:
                            endDate = endDate.AddDays(myArray[5, 1]);
                            break;
                        case System.DayOfWeek.Sunday:
                            endDate = endDate.AddDays(myArray[6, 1]);
                            break;
                    }

                    if (endDate.DayOfWeek == listSelected.First())
                        //Last selected Day of the week is reached
                        endDate = endDate.AddDays((interval.WeekInterval - 1) * 7); // Every x weeks ( = 7 days)

                    occurrences++;
                }

                interval.EndDate = endDate;
            }
            else if (intervalEndType == RecurrenceEndTypeEnum.EndByDate)
            {
                // Solution 2: count as far until EndByDate is reached/overstepped
                var endDate = interval.StartDate.Date;
                var previousEndDate = endDate;

                // Calculation of End Date based on user's settings
                while (endDate.Date <= interval.EndDate)
                {
                    previousEndDate =
                        endDate; // Previous calculated date gets stored before the next date gets calculated

                    endDate = endDate.AddDays(1);
                    switch (endDate.DayOfWeek)
                    {
                        case System.DayOfWeek.Monday:
                            endDate = endDate.AddDays(myArray[0,
                                1]); // add so many days that it needs to get to the next selected day.
                            break;
                        case System.DayOfWeek.Tuesday:
                            endDate = endDate.AddDays(myArray[1, 1]);
                            break;
                        case System.DayOfWeek.Wednesday:
                            endDate = endDate.AddDays(myArray[2, 1]);
                            break;
                        case System.DayOfWeek.Thursday:
                            endDate = endDate.AddDays(myArray[3, 1]);
                            break;
                        case System.DayOfWeek.Friday:
                            endDate = endDate.AddDays(myArray[4, 1]);
                            break;
                        case System.DayOfWeek.Saturday:
                            endDate = endDate.AddDays(myArray[5, 1]);
                            break;
                        case System.DayOfWeek.Sunday:
                            endDate = endDate.AddDays(myArray[6, 1]);
                            break;
                    }

                    if (endDate.DayOfWeek == listSelected.First())
                        // Last selected Day of the week is reached
                        endDate = endDate.AddDays((interval.WeekInterval - 1) * 7); // Every x weeks ( = 7 days)
                }

                interval.EndDate = previousEndDate;
            }

            // Add the time to the dates.
            interval.StartDate = interval.StartDate.Date + recurrableItem.StartsAt.TimeOfDay;
            interval.EndDate = interval.EndDate.Date + recurrableItem.EndsAt.TimeOfDay;
        }

        private void _CalculateDateRangeForYearlyIntervals(IRecurrable recurrableItem, RecurringIntervalBase interval)
        {
            interval.StartDate = recurrableItem.StartsAt.Date;

            var intervalEndType = (RecurrenceEndTypeEnum)interval.EndType;
            if (intervalEndType != RecurrenceEndTypeEnum.EndByDate)
                interval.EndDate = recurrableItem.EndsAt;

            var intervalEndTypeIsSet = intervalEndType == RecurrenceEndTypeEnum.NoEndDate
                                       || intervalEndType == RecurrenceEndTypeEnum.EndAfterOccurrences
                                       || intervalEndType == RecurrenceEndTypeEnum.EndByDate;

            // BEGIN DATE: Calculation (if adjustment is necessary)
            if (intervalEndTypeIsSet)
            {
                // Calculation of Begin Date(adjusted) and End Date based on user's settings
                var startDate = interval.StartDate.Date;
                var month = interval.Month;
                var day = interval.DayOfMonth;
                var adjustedDate = new DateTime(startDate.Year, month, day);

                if (adjustedDate > startDate) // stay in the same selected year
                    startDate = new DateTime(startDate.Year, month, day); // set calculated begin date
                else if (adjustedDate < startDate) // start with next month
                    startDate = new DateTime(startDate.Year + 1, month, day);

                interval.StartDate = startDate;
            }

            // END DATE: Calculation based on user's settings (occurence, day interval)
            if (intervalEndType == RecurrenceEndTypeEnum.NoEndDate)
            {
                interval.EndDate = interval.StartDate.Date;
            }
            else if (intervalEndType == RecurrenceEndTypeEnum.EndAfterOccurrences)
            {
                var endDate = interval.StartDate.Date;
                endDate = endDate.AddYears(interval.EndOccurs - 1);
                interval.EndDate = endDate;
            }
            else if (intervalEndType == RecurrenceEndTypeEnum.EndByDate)
            {
                var endDate = interval.StartDate.Date;

                while (endDate.Date <= interval.EndDate.Date)
                    endDate = endDate.AddYears(1);

                endDate = endDate.AddYears(-1);

                interval.EndDate = endDate;
            }

            // Set the correct time for the start and end dates
            interval.StartDate = interval.StartDate.Date + recurrableItem.StartsAt.TimeOfDay;
            interval.EndDate = interval.EndDate.Date + recurrableItem.EndsAt.TimeOfDay;
        }

        public Func<DateTime, DateTime> BuildIntervalDateIterator()
        {
            switch ((IntervalTypeEnum)IntervalType)
            {
                /********************************************
				 * Daily
				 */
                case IntervalTypeEnum.Daily:
                {
                    return currentDate =>
                    {
                        switch ((DailyIntervalSubTypeEnum)SubType)
                        {
                            case DailyIntervalSubTypeEnum.NthDay:
                            {
                                // Add N days to the current date.
                                return currentDate.AddDays(DayInterval);
                            }
                            case DailyIntervalSubTypeEnum.Weekday:
                            {
                                // Advance by the number of days necessary to get to the next weekday.
                                var dayOfWeek = currentDate.DayOfWeek;
                                if (dayOfWeek >= System.DayOfWeek.Friday)
                                    return currentDate.AddDays(2 + System.DayOfWeek.Saturday - dayOfWeek);
                                return currentDate.AddDays(1);
                            }
                            case DailyIntervalSubTypeEnum.TuesThurs:
                            {
                                // Advance by the number of days necessary to get to the next TuTh.
                                var dayOfWeek = currentDate.DayOfWeek;
                                if (dayOfWeek < System.DayOfWeek.Tuesday)
                                    return currentDate.AddDays(System.DayOfWeek.Tuesday - dayOfWeek);
                                if (dayOfWeek < System.DayOfWeek.Thursday)
                                    return currentDate.AddDays(System.DayOfWeek.Thursday - dayOfWeek);
                                return
                                    currentDate.AddDays(1 + System.DayOfWeek.Saturday - dayOfWeek +
                                                        (int)System.DayOfWeek.Tuesday);
                            }
                            case DailyIntervalSubTypeEnum.MonWedFri:
                            {
                                // Advance by the number of days necessary to get to the next MWF.
                                var dayOfWeek = currentDate.DayOfWeek;
                                if (dayOfWeek < System.DayOfWeek.Monday)
                                    return currentDate.AddDays(System.DayOfWeek.Monday - dayOfWeek);
                                if (dayOfWeek < System.DayOfWeek.Wednesday)
                                    return currentDate.AddDays(System.DayOfWeek.Wednesday - dayOfWeek);
                                if (dayOfWeek < System.DayOfWeek.Friday)
                                    return currentDate.AddDays(System.DayOfWeek.Friday - dayOfWeek);
                                return
                                    currentDate.AddDays(1 + System.DayOfWeek.Saturday - dayOfWeek +
                                                        (int)System.DayOfWeek.Monday);
                            }
                            default:
                            {
                                return DateTime.MaxValue;
                            }
                        }
                    };
                }

                /********************************************
				 * Weekly
				 */
                case IntervalTypeEnum.Weekly:
                {
                    // Prepare a lookup to simplify finding the next day of week for occurrences.
                    var selectedDaysOfWeek = new[]
                    {
                        IsSundaySet,
                        IsMondaySet,
                        IsTuesdaySet,
                        IsWednesdaySet,
                        IsThursdaySet,
                        IsFridaySet,
                        IsSaturdaySet
                    };

                    return currentDate =>
                    {
                        // Determine the value of the next day of the week. 
                        var currentDayOfWeek = (int)currentDate.DayOfWeek;
                        var nextDayOfWeek = 0;
                        for (var i = 1; i <= 7; i++)
                        {
                            var day = (currentDayOfWeek + i) % 7;
                            if (selectedDaysOfWeek[day])
                            {
                                nextDayOfWeek = day;
                                break;
                            }
                        }

                        // Calculate the number of days to advance.
                        int daysToAdd;
                        if (currentDayOfWeek == nextDayOfWeek)
                            daysToAdd = 7 * WeekInterval;
                        else if (currentDayOfWeek > nextDayOfWeek)
                            // Advance to the next day in the next week, then apply the week interval.
                            // Have to offset the interval by 1 since we will already be in the next week.
                            daysToAdd = 7 - currentDayOfWeek + nextDayOfWeek
                                                             + 7 * (WeekInterval - 1);
                        else // currentDayOfWeek < nextDayOfWeek
                            daysToAdd = nextDayOfWeek - currentDayOfWeek;

                        return currentDate.AddDays(daysToAdd);
                    };
                }

                /********************************************
				 * Monthly
				 */
                case IntervalTypeEnum.Monthly:
                {
                    return currentDate =>
                    {
                        switch ((MonthlyIntervalSubTypeEnum)SubType)
                        {
                            case MonthlyIntervalSubTypeEnum.DayOfMonth:
                            {
                                // Move forward N months.
                                var nextDate = currentDate.AddMonths(MonthInterval);
                                var i = 1;

                                // If the day of the next date is not correct (will happen with 31/30/29 days),
                                // then keep moving forward along the set interval until the day is correct.
                                while (currentDate.Day != nextDate.Day)
                                    nextDate = currentDate.AddMonths(MonthInterval * i++);

                                return nextDate;
                            }
                            case MonthlyIntervalSubTypeEnum.DayOfWeek:
                            {
                                // Calculate the day of week the next month starts on.
                                var nextMonth = currentDate.AddMonths(MonthInterval);
                                var firstDayOfNextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);
                                var firstDayOfNextMonthDayOfWeek = (int)firstDayOfNextMonth.DayOfWeek;
                                var intervalDayOfWeek = DayOfWeek % 7;

                                // Get the first relevant day of week in the next month.
                                // Branch on whether the first interval day for the month is in the second week.
                                var firstIntervalDayInNextMonth
                                    = firstDayOfNextMonthDayOfWeek > intervalDayOfWeek
                                        ? firstDayOfNextMonth.AddDays(7 - firstDayOfNextMonthDayOfWeek +
                                                                      intervalDayOfWeek)
                                        : firstDayOfNextMonth.AddDays(intervalDayOfWeek - firstDayOfNextMonthDayOfWeek);

                                // Advance the date to the Nth day of week.
                                // If we overshoot, then rewind by a week.
                                var nextDate = firstIntervalDayInNextMonth.AddDays(7 * (DayQualifier - 1));
                                if (nextDate.Month != nextMonth.Month) nextDate = nextDate.AddDays(-7);

                                return nextDate;
                            }
                            default:
                            {
                                return DateTime.MaxValue;
                            }
                        }
                    };
                }

                /********************************************
				 * Yearly
				 */
                case IntervalTypeEnum.Yearly:
                {
                    return currentDate =>
                    {
                        // Advance a year, and test that the month and day exist in the year.
                        // There could be a booboo if the interval is to repeat on a leap year.
                        var nextDate = currentDate.AddYears(1);
                        if (Month == 2 && DayOfMonth == 29)
                            while (nextDate.Month != Month || nextDate.Day != DayOfMonth)
                                nextDate = currentDate.AddYears(1);
                        return nextDate;
                    };
                }

                /********************************************
				 * Just in case something goes wrong....
				 */
                default:
                {
                    return date => DateTime.MaxValue;
                }
            }
        }

        public void CalculateDateRange(IRecurrable recurrableItem)
        {
            var interval = this;

            switch ((IntervalTypeEnum)interval.IntervalType)
            {
                case IntervalTypeEnum.Daily:
                    _CalculateDateRangeForDailyIntervals(recurrableItem, interval);
                    break;
                case IntervalTypeEnum.Weekly:
                    _CalculateDateRangeForWeeklyIntervals(recurrableItem, interval);
                    break;
                case IntervalTypeEnum.Monthly:
                    _CalculateDateRangeForMonthlyIntervals(recurrableItem, interval);
                    break;
                case IntervalTypeEnum.Yearly:
                    _CalculateDateRangeForYearlyIntervals(recurrableItem, interval);
                    break;
            }
        }


        public object Clone()
        {
            return new RecurringInterval
            {
                IntervalType = IntervalType,
                SubType = SubType,
                DayInterval = DayInterval,
                WeekInterval = WeekInterval,
                MonthInterval = MonthInterval,
                DayOfMonth = DayOfMonth,
                DayOfWeek = DayOfWeek,
                DayQualifier = DayQualifier,
                IsSundaySet = IsSundaySet,
                IsMondaySet = IsMondaySet,
                IsTuesdaySet = IsTuesdaySet,
                IsWednesdaySet = IsWednesdaySet,
                IsThursdaySet = IsThursdaySet,
                IsFridaySet = IsFridaySet,
                IsSaturdaySet = IsSaturdaySet,
                Month = Month,
                EndType = EndType,
                EndOccurs = EndOccurs,
                EndDate = EndDate,
                StartDate = StartDate
            };
        }

        public IEnumerable<RecurringIntervalOccurrence> GetAllOccurrences()
        {
            // We're going to stop yielding occurrences by the given end date if this interval has 
            // an unset or never-ending end type. If the interval ends by a specific date
            // (either cause it's an "end after" or "by end date" type), then use the earliest end date.
            var maxDate = DateTime.MaxValue;
            var maxOccurrenceNumber = int.MaxValue;
            var endType = (RecurrenceEndTypeEnum)EndType;
            switch (endType)
            {
                case RecurrenceEndTypeEnum.EndAfterOccurrences:
                    maxOccurrenceNumber = EndOccurs;
                    break;

                case RecurrenceEndTypeEnum.EndByDate:
                    maxDate = EndDate;
                    break;

                default: /* NotSet and NoEndDate */
                    throw new InvalidDataException("Recurrence End Type is invalid: " + endType);
            }

            // Iterate from the start of the interval up to the max end date or occurrence number.
            var occurrences = new List<RecurringIntervalOccurrence>();
            var currentDate = StartDate;
            var getNextDate = BuildIntervalDateIterator();
            var iOccurrenceNumber = 1;
            while (currentDate <= maxDate && iOccurrenceNumber <= maxOccurrenceNumber)
            {
                occurrences.Add(new RecurringIntervalOccurrence
                {
                    StartsAt = currentDate.SetTime(StartDate),
                    EndsAt = currentDate.SetTime(EndDate),
                    OccurrenceNumber = iOccurrenceNumber++
                });

                currentDate = getNextDate(currentDate);
            }

            ProcessDeletedOccurrences(occurrences);

            return occurrences;
        }

        public IEnumerable<RecurringIntervalOccurrence> GetOccurrences(DateTime startDate, DateTime endDate,
            DateTime? endsAt = null)
        {
            // We're going to stop yielding occurrences by the given end date if this interval has 
            // an unset or never-ending end type. If the interval ends by a specific date
            // (either cause it's an "end after" or "by end date" type), then use the earliest end date.
            DateTime maxDate;
            var maxOccurrenceNumber = int.MaxValue;
            var endType = (RecurrenceEndTypeEnum)EndType;
            switch (endType)
            {
                case RecurrenceEndTypeEnum.EndAfterOccurrences:
                    maxDate = endDate;
                    maxOccurrenceNumber = EndOccurs;
                    break;

                case RecurrenceEndTypeEnum.EndByDate:
                    maxDate = endDate < EndDate ? endDate : EndDate;
                    break;

                default: /* NotSet and NoEndDate */
                    maxDate = endDate;
                    break;
            }

            // Iterate from the start of the interval up to the max end date or occurrence number.
            var occurrences = new List<RecurringIntervalOccurrence>();
            var currentDate = StartDate;
            var getNextDate = BuildIntervalDateIterator();
            var iOccurrenceNumber = 1;
            while (currentDate <= maxDate && iOccurrenceNumber <= maxOccurrenceNumber)
            {
                occurrences.Add(new RecurringIntervalOccurrence
                {
                    StartsAt = currentDate.SetTime(StartDate),
                    //EndsAt = currentDate.SetTime(EndDate),
                    EndsAt = endsAt == null ? currentDate.SetTime(EndDate) : currentDate.SetTime(endsAt),
                    OccurrenceNumber = iOccurrenceNumber++
                });

                currentDate = getNextDate(currentDate);
            }

            ProcessDeletedOccurrences(occurrences);

            // Return all occurrences after the given start date.
            return occurrences.Where(o => o.StartsAt >= startDate);
        }

        public virtual void ProcessDeletedOccurrences(List<RecurringIntervalOccurrence> occurrences)
        {
        }
    }
}