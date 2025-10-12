using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class ImageDocumentType : IUpdateAudited
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string AssociatedExtension { get; set; }
        public DateTime CreatedDate { get; set; }
        public byte[] RowVersion { get; set; }
        public int DocumentQuality { get; set; }
        public string ScannerSettings { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        
    }
}
