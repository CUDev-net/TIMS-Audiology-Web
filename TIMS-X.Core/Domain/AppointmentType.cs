using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class AppointmentType : Entity, IUpdateAudited, ICreateDateAudited
    {
        public bool Inactive { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Protected { get; set; }
        public int Duration { get; set; }
        public int InUse { get; set; }
        public int? Color { get; set; }
        public int? ScheduleBlockId { get; set; }
        public int? HistoryTypeId { get; set; }
        public bool Slp { get; set; }
        public virtual ScheduleBlock ScheduleBlock { get; set; }
        public bool SendsNotification { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}