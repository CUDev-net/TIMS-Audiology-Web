using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain.Imaging
{
    public class TimsImage
    {
        public Guid Id { get; set; }
        public Guid DocumentTypeId { get; set; }
        public int UpdatedUserId { get; set; }
        public DateTime DateCreated { get; set; }
        public byte[] Image { get; set; }
        public byte[] RowVersion { get; set; }
        public bool IsEncrypted { get; set; }
        public bool IsCompressed { get; set; }
    }
}
