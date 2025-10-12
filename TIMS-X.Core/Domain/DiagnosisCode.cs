using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class DiagnosisCode : Entity, IUpdateDateAudited
    {
        public bool Inactive { get; set; }
        public int InUse { get; set; }
        public bool Superbill { get; set; }
        public int Icd10Status { get; set; }
        public bool IsIcd10 { get; set; }
        public int UpdatedUserId { get; set; }
        public string Name { get; set; }
        public bool Protected { get; set; }
        public string Description { get; set; }
        public DiagnosisCodeCategory Category { get; set; }
        public int CategoryId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}