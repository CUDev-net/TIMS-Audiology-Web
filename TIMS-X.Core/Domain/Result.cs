using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class Result : Entity, IUpdateAudited
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Inactive { get; set; }
        public int? InUse { get; set; }
        public bool Protected { get; set; }
        public int? ResultTypeId { get; set; }
        public ResultType ResultType { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
    }
}