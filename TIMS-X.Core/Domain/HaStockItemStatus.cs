using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class HaStockItemStatus : Entity, IUpdateAudited
    {
        public bool Inactive { get; set; }
        public string Name { get; set; }
        public bool Protected { get; set; }
        public string Description { get; set; }
        public bool AvailableForSale { get; set; }
        public HaStatus HaStatus { get; set; }
        public int? StatusId { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}