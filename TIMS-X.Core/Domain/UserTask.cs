using System;
using TIMS_X.Core.Attributes;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class UserTask : Entity, ICreateDateAudited
    {
        public int? UpdatedUserId { get; set; }
        public int? MadeBy { get; set; }
        public string Task { get; set; }
        public int? UserTaskTypeId { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? DueDate { get; set; }
        [TimsObject] public UserTaskType UserTaskType { get; set; }

        public int? PatientId { get; set; }
        public Guid? RecurringTaskId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}