using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class HaOrderImage : ICreateByUserAudited, ICreateDateAudited
    {
        public Guid Id { get; set; }
        public Guid ImageServerId { get; set; }
        public Guid DocumentTypeId { get; set; }
        public int HaOrderId { get; set; }
        public Guid ImageId { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public byte[] RowVersion { get; set; }

    }
}
