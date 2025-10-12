using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain.Base;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Domain
{
    public class Alert : ICreateByUserAudited, ICreateDateAudited
    {
        public int Id { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DueDate { get; set; }
        public int AlertUserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public AlertTypeEnum AlertType { get; set; }
        public int AlertObjectId { get; set; }
    }
}
