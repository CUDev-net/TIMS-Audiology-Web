using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class BatterySize : Entity, IUpdateAudited
    {
        public bool Inactive { get; set; }
        public int InUse { get; set; }
        public bool Taxable { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool Protected { get; set; }
        public string QbId { get; set; }
        public DateTime? QbModifiedDate { get; set; }
        public string Description { get; set; }
        public bool IsTaxIncluded { get; set; }
        public int TaxGroupId { get; set; }
        public string QbAcctId { get; set; }
        public int PosItemTypeId { get; set; }
        public bool SuperBill { get; set; }
        public int QbTypeId { get; set; }
        public bool? Inventory { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
