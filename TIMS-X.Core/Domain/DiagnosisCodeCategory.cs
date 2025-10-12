using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class DiagnosisCodeCategory : Entity, IUpdateDateAudited, ICreateDateAudited
    {
        public bool Inactive { get; set; }
        public int UpdatedUserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}