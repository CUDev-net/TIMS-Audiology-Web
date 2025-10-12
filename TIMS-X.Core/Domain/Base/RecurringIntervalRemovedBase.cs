using System;
using System.Collections.Generic;

namespace TIMS_X.Core.Domain.Base
{
    public class RecurringIntervalRemovedBase : Entity
    {
        public int RecurringIntervalId { get; set; }
        public int ItemNumber { get; set; }
        public DateTime? ItemDate { get; set; }
    }
}
