using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class UserSiteHours : Entity
    {
        public int UserId { get; set; }
        public int SiteId { get; set; }
        public int DayNum { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public Site Site { get; set; }
    }
}
