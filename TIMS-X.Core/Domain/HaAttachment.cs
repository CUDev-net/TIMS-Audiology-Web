using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class HaAttachment : IUpdateAudited, ICreateByUserAudited, ICreateDateAudited
    {
        public int Id { get; set; }
        public bool Inactive { get; set; }
        public int UpdatedSiteId { get; set; }
        public int HaHistoryId { get; set; }
        public int HaAttachmentTypeId { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? WarrantyDate { get; set; }
        public string Notes { get; set; }
        public int? StatusId { get; set; }
        public string ManufactureModel { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string SerialNumber { get; set; }
        public string Invoice { get; set; }
        public string UdiNumber { get; set; }
        public decimal Cost { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
