using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class Modifier : Entity, IUpdateDateAudited
    {
        public bool Inactive { get; set; }
        public string Name { get; set; }
        public int InUse { get; set; }
        public bool Protected { get; set; }
        public int UpdatedUserId { get; set; }
        public string Description { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}