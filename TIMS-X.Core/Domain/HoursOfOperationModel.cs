using System;

namespace TIMS_X.Core.Domain
{
/*
enum day_of_week {
sunday,
monday,
tuesday,
wednesday,
thursday,
friday,
saturday
}

export class HoursOfOperationModel { 
    public startTime: Date;
    public endTime: Date;
    public day: day_of_week
    public siteId: number
}
    
*/
    public class HoursOfOperationModel
    {
        public HoursOfOperationModel()
        {
        }

        public HoursOfOperationModel(DayOfWeek day, DateTime? startTime, DateTime? endTime)
        {
            Day = day;
            StartTime = startTime;
            EndTime = endTime;
        }

        public DayOfWeek Day { get; set; }

        public DateTime? EndTime { get; set; }

        /// <summary>
        ///     Providers have hours per site, so this is here to track that condition.
        /// </summary>
        public int? SiteId { get; set; }

        public DateTime? StartTime { get; set; }
    }
}