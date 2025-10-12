using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class HaOrder : Entity, IUpdateAudited, ICreateByUserAudited, ICreateDateAudited
    {
        public int SyncSiteId { get; set; }
        public int HaHistoryId { get; set; }
        public int PatientId { get; set; }
        public int ManufacturerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedInDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string OrderNumber { get; set; }
        public string TrackingNumber { get; set; }
        public string Notes { get; set; }
        public bool IsVoided { get; set; }
        public int EOrderStatus { get; set; }
        public int UpdatedSiteId { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public string PdfOrderNumber { get; set; }
        public string PdfOrderData { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}