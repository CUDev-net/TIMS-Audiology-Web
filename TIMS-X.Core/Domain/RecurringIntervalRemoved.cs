using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class RecurringIntervalRemoved : RecurringIntervalRemovedBase
    {
        public int ScheduleId { get; set; }
    }
}
