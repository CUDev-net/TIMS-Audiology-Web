using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain
{
    public class PatientImage
    {
        public Guid Id { get; set; }
        public Guid ImageServerId { get; set; }
        public Guid DocumentTypeId { get; set; }
        public Guid PatientGuid { get; set; }
        public Guid ImageId { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public byte[] RowVersion { get; set; }
        public string Password { get; set; }
        public DateTime? DtExpires { get; set; }
        public bool WebAccess { get; set; }
        public byte[] Image { get; set; }
    }
}
