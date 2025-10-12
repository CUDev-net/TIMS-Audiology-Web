using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain.Base;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Domain
{
    public class PatientReportTemplate : IUpdateAudited
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PatientReportCategoryEnum ReportCategory { get; set; }
        public byte[] ReportTemplate { get; set; }
        public bool Inactive { get; set; }
        public bool Protected { get; set; }
        public int AutoArchive { get; set; }
        public bool Editable { get; set; }
        public int? UpdatedUserId { get; set; }
    }
}
