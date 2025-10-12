using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TIMS_X.Core.Domain.Base;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Extensions;
using TIMS_X.Core.Models;

namespace TIMS_X.Core.Domain
{
    public enum RecurrenceEndTypeEnum
    {
        NotSet = 0,
        NoEndDate,
        EndAfterOccurrences,
        EndByDate
    }

    public class RecurringInterval : RecurringIntervalBase, ICloneable
    {
        public RecurringInterval()
        {
            DeletedOccurrences = new HashSet<RecurringIntervalRemoved>();
        }

        public ICollection<RecurringIntervalRemoved> DeletedOccurrences { get; set; }

        public override void ProcessDeletedOccurrences(List<RecurringIntervalOccurrence> occurrences)
        {
            var deletedOccurrenceNumbers = DeletedOccurrences.Select(x => x.ItemNumber).ToList();
            occurrences.RemoveAll(o => deletedOccurrenceNumbers.Contains(o.OccurrenceNumber));
        }
    }
}