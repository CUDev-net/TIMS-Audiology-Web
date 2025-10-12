using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain.Noah
{
    public class N4DashboardAlertArchive
    {
        public Guid AlertGuid { get; set; }
        public int? NotificationActionId { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public Guid PatientGuid { get; set; }
        public int? ActionId { get; set; }
        public string Url { get; set; }
        public int? ModuleId { get; set; }
        public string ModuleParameter { get; set; }
        public Guid? Group { get; set; }
        public int AppModuleId { get; set; }
        public DateTime ReceivedUtc { get; set; }
        public DateTime LastModifiedUtc { get; set; }
        public int Priority { get; set; }
        public int? AssigneeUserId { get; set; }
        public bool IsRead { get; set; }
        public string IconUrl { get; set; }
    }
}
