using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class HaReturn : Entity, IUpdateAudited
    {
        public int SyncSiteId { get; set; }
        public int HaHistoryId { get; set; }
        public int PatientId { get; set; }
        public int HaReturnReasonId { get; set; }
        public DateTime ReturnDate { get; set; }
        public bool ReturnedToManufacturer { get; set; }
        public DateTime? ReturnedToManufacturerDate { get; set; }
        public string Notes { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }

        public HaReturnReason ReturnReason { get; set; }
    }
}
