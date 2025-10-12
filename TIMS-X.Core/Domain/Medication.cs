using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class Medication : Entity, IUpdateAudited, ICreateDateAudited
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Inactive { get; set; }
        public bool IsOtotoxic { get; set; }
        public bool ForDryMouth { get; set; }
        public bool ForPsychologicalIssuesCausingStutter { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
    }
}