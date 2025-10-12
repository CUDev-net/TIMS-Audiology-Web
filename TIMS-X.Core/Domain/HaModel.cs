using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class HaModel : Entity, IUpdateAudited
    {
        public bool Inactive { get; set; }
        public int InUse { get; set; }
        public string Name { get; set; }
        public int ManufacturerId { get; set; }
        public int BatterySizeId { get; set; }
        public int HaStyleId { get; set; }
        public int HaTypeId { get; set; }
        public decimal Price { get; set; }
        public bool Taxable { get; set; }
        public bool Protected { get; set; }
        public int WarrantyPeriod { get; set; }
        public string Description { get; set; }
        public string QbId { get; set; }
        public DateTime? QbModifiedDate { get; set; }
        public bool? IsAccessory { get; set; }
        public bool IsTaxIncluded { get; set; }
        public int TaxGroupId { get; set; }
        public bool IsEarmold { get; set; }
        public string QbAcctId { get; set; }
        public int PosItemTypeId { get; set; }
        public bool SuperBill { get; set; }
        public int QbTypeId { get; set; }
        public bool? Inventory { get; set; }
        public bool? AvailableForSale { get; set; }
        public int CatalogModelId { get; set; }
        public int WarrantyPeriodDays { get; set; }
        public decimal Cost { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public BatterySize BatterySize { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public HaStyle Style { get; set; }
        public HaType HaType { get; set; }
    }
}
