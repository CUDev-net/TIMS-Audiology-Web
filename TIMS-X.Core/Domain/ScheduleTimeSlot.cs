using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class ScheduleTimeSlot : Entity
    {
        public int DayOfWeek { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedDate { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
