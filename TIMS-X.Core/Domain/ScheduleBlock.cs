using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class ScheduleBlock : Entity, IUpdateAudited
    {
        public ScheduleBlock()
        {
            ProviderBlockSchedules = new HashSet<ProviderBlockSchedule>();
            AppointmentTypes = new List<string>();
        }

        public DateTime CreatedDate { get; set; }
        public string Name { get; set; }
        public int Color { get; set; }
        public string color_web { get; set; }
        public string Notes { get; set; }
        public byte[] RowVersion { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<string> AppointmentTypes { get; }
        public bool Inactive { get; set; }

        public ICollection<ProviderBlockSchedule> ProviderBlockSchedules { get; set; }
    }
}
