using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class HaStockItem : IUpdateAudited
    {
        public int Id { get; set; }
        public int HaModelId { get; set; }
        public int SiteId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Side { get; set; }
        public DateTime ReceivedDate { get; set; }
        public decimal Cost { get; set; }
        public string SerialNumber { get; set; }
        public string OrderNumber { get; set; }
        public string Notes { get; set; }
        public string QbId { get; set; }
        public DateTime? QbUpdatedDate { get; set; }
        public bool? Available { get; set; }
        public bool Inactive { get; set; }
        public string InactiveReason { get; set; }
        public byte[] RowVersion { get; set; }
        public string PoNumber { get; set; }
        public int Status { get; set; }
        public decimal Price { get; set; }
        public bool Loaner { get; set; }
        public string UdiNumber { get; set; }
        public int HaStyleId { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
