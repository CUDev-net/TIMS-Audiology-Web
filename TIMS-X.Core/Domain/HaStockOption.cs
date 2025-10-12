using System;
using System.Collections.Generic;

namespace TIMS_X.Core.Domain
{
    public class HaStockOption
    {
        public int Id { get; set; }
        public int? HaStockItemId { get; set; }
        public int? HaOptionId { get; set; }
    }
}
