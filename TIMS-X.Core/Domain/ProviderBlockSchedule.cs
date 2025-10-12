using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class ProviderBlockSchedule : Entity
    {
        public int ProviderId { get; set; }
        public int ScheduleBlockId { get; set; }
        public int ScheduleTimeSlotId { get; set; }
        public DateTime CreatedDate { get; set; }
        public byte[] RowVersion { get; set; }
        public ScheduleBlock ScheduleBlock { get; set; }
        public ScheduleTimeSlot ScheduleTimeSlot { get; set; }
    }
}
