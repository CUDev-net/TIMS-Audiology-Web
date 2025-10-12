using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain
{
    public class PatientLetterArchive
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedByUserId { get; set; }
        public string Name { get; set; }
        public DateTime PrintedDate { get; set; }
        public int LetterCategory { get; set; }
        public int PatientId { get; set; }
        public int TemplateId { get; set; }
        public int ArchiveId { get; set; }
        public Guid ArchiveServerId { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string EmailSubject { get; set; }
        public bool WebAccess { get; set; }
        public string Password { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
