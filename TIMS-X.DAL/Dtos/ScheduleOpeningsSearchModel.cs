using System;
using System.Collections.Generic;

namespace TIMS_X.DAL.Dtos
{
    public class ScheduleOpeningsSearchModel
    {
        public ScheduleOpeningsSearchModel()
        {
            ProviderIds = new List<int>();
            SiteIds = new List<int>();
            ResourceIds = new List<int>();
        }
        
        public int? AppointmentTypeId { get; set; }

        public int? DurationHours { get; set; }

        public int? DurationMinutes { get; set; }

        public int DurationTotalMinutes { get; set; }

        public DateTime? EndDate { get; set; }

        public IEnumerable<int> ProviderIds { get; set; }

        public IEnumerable<int> ResourceIds { get; set; }

        public IEnumerable<int> SiteIds { get; set; }

        public DateTime? StartDate { get; set; }
    }
}