using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class HistoryType : Entity, IUpdateAudited
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsSlp { get; set; }
        public string ReportExportName { get; set; }
        public bool Inactive { get; set; }
        public int InUse { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}