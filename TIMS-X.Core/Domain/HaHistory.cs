using System;
using TIMS_X.Core.Domain.Base;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Domain
{
    public class HaHistory : Entity, IUpdateAudited, ICreateByUserAudited, ICreateDateAudited
    {
        public int SyncSiteId { get; set; }
        public int UpdatedSiteId { get; set; }
        public int PatientId { get; set; }
        public int HaModelId { get; set; }
        public int ProviderId { get; set; }
        public HaSide Side { get; set; }
        public int OtherSideId { get; set; }
        public bool IsEarmold { get; set; }
        public bool AddInvoice { get; set; }
        public int BatterySizeId { get; set; }
        public int HaStatusId { get; set; }
        public int WarrantyTypeId { get; set; }
        public DateTime? FitDate { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public DateTime? OrigWarrantyDate { get; set; }
        public DateTime? WarrantyDate { get; set; }
        public DateTime? LossDamageDate { get; set; }
        public string SerialNumber { get; set; }
        public string Invoice { get; set; }
        public int? PosDocId { get; set; }
        public string QbInvoice { get; set; }
        public DateTime? QbUpdateDate { get; set; }
        public string Notes { get; set; }
        public int AppointmentId { get; set; }
        public bool IsAccessory { get; set; }
        public int HaStockItemId { get; set; }
        public bool AddCrMemo { get; set; }
        public int CrMemoId { get; set; }
        public int? OriginalId { get; set; }
        public int? HaOrderId { get; set; }
        public bool IsCros { get; set; }
        public bool ThirdPartyAid { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int DiscountId { get; set; }
        public bool Combined { get; set; }
        public decimal Cost { get; set; }
        public bool Loaner { get; set; }
        public string UdiNumber { get; set; }
        public int LdWarrantyTypeId { get; set; }
        public int HaStyleId { get; set; }
        public HaStatus Status { get; set; }
        public HaModel Model { get; set; }
        public Patient Patient { get; set; }
        public Provider Provider { get; set; }
        public HaStockItem HaStockItem { get; set; }
        public HaOrder HaOrder { get; set; }
        public HaStyle HaStyle { get; set; }
        public Appointment Appointment { get; set; }
        public BatterySize BatterySize { get; set; }
        public HaWarrantyType WarrantyType { get; set; }
        public Site Site { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}