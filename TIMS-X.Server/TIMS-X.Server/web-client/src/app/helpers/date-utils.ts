import { DateTime } from "luxon";

export module DateUtils {

    export abstract class Constants {
        static readonly API_DATE_FORMAT = "yyyy-LL-dd'T'HH:mm:ss";
    }

    export function createApiDateTime(datePart: any, timePart: any): any {
        /// <summary>Returns a new date with the date and time.</summary>
        /// <param name="date">The date component; can be a string, date, or moment</param>
        /// <param name="time">The time component; can be a string, date, or moment</param>

        return createDateTime(datePart, timePart).toFormat(Constants.API_DATE_FORMAT);
    }

    export function createDateTime(datePart: any, timePart: any): any {
        /// <summary>Returns a new date with the date and time.</summary>
        /// <param name="date">The date component; can be a string, date, or moment</param>
        /// <param name="time">The time component; can be a string, date, or moment</param>

        var date = DateTime.fromJSDate(datePart);
        var time = DateTime.fromJSDate(timePart);

        return date.set({
            hour: timePart.getHours(),
            minute: timePart.getMinutes(),
            second: 0
        });
    }

    export function createDbDateOnlyFromDate(date) {
        if (date)
            return new Date(date.getFullYear(), date.getMonth(), date.getDate());
        return null;
    }
}