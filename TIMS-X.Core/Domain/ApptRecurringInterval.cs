using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;
using System.Linq;
using TIMS_X.Core.Models;

namespace TIMS_X.Core.Domain
{
    public class ApptRecurringInterval : RecurringIntervalBase
    {
        public ApptRecurringInterval()
        {
            DeletedOccurrences = new HashSet<ApptRecurringIntervalRemoved>();
        }

        public ICollection<ApptRecurringIntervalRemoved> DeletedOccurrences { get; set; }

        public override void ProcessDeletedOccurrences(List<RecurringIntervalOccurrence> occurrences)
        {
            var deletedOccurrenceNumbers = DeletedOccurrences.Select(x => x.ItemNumber).ToList();
            occurrences.RemoveAll(o => deletedOccurrenceNumbers.Contains(o.OccurrenceNumber));
        }
    }
}
