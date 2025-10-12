using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class HaLoaner : IUpdateAudited, ICreateByUserAudited, ICreateDateAudited
    {
        public int Id { get; set; }
        public int SyncSiteId { get; set; }
        public int HaHistoryId { get; set; }
        public int PatientId { get; set; }
        public int ManufacturerId { get; set; }
        public int HaModelId { get; set; }
        public string SerialNumber { get; set; }
        public DateTime OutDate { get; set; }
        public DateTime DueBackDate { get; set; }
        public DateTime? InDate { get; set; }
        public string Notes { get; set; }
        public int HaStockItemId { get; set; }
        public int UpdatedSiteId { get; set; }
        public string UdiNumber { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
