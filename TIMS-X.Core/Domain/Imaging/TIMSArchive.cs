using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain.Imaging
{
    public class TimsArchive
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedUserId { get; set; }
        public int ArchiveType { get; set; }
        public byte[] ArchiveImage { get; set; }
        public string ArchiveHtml { get; set; }
        public int? ArchiveTemplateId { get; set; }
        public string ArchiveTemplateData { get; set; }
    }
}
