using System;

namespace TIMS_X.BLL.Utilities;

public static class DayOfWeekTranslator
{
    public static int GetDayOfWeekAsNumber(this DateTime date)
    {
        int daynumber;
        switch (date.DayOfWeek)
        {
            case DayOfWeek.Monday:
                daynumber = 1;
                break;
            case DayOfWeek.Tuesday:
                daynumber = 2;
                break;
            case DayOfWeek.Wednesday:
                daynumber = 3;
                break;
            case DayOfWeek.Thursday:
                daynumber = 4;
                break;
            case DayOfWeek.Friday:
                daynumber = 5;
                break;
            case DayOfWeek.Saturday:
                daynumber = 6;
                break;
            default:
                daynumber = 7;
                break;
        }

        return daynumber;
        ;
    }
}