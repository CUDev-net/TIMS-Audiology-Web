using System;

namespace TIMS_X.Core.Models
{
    public class RecurringIntervalOccurrence
    {
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public int OccurrenceNumber { get; set; }

    }
}
