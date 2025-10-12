using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class User : Entity, IUpdateAudited
    {
        public User()
        {
            SiteHours = new HashSet<UserSiteHours>();
        }

        public bool Inactive { get; set; }
        public string Name { get; set; }
        public string Initials { get; set; }
        public string AdDomain { get; set; }
        public string AdUsername { get; set; }
        public bool IsAdmin { get; set; }
        public bool Deleted { get; set; }
        public bool IsWebUser { get; set; }
        public int ColorFrom { get; set; }
        public int CalendarInterval { get; set; }
        public string Password { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public bool RequireMFA { get; set; }
        public int SiteId { get; set; }
        public string ScheduleProviderFilter { get; set; }
        public string ScheduleSiteFilter { get; set; }
        public string ScheduleResourceFilter { get; set; }
        public string ScheduleSpecialtyFilter { get; set; }
        public DateTime? PasswordChangedDate { get; set; }
        public string UserSettings { get; set; }
        public string Jwt { get; set; }

        public int NoahPermissions { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }

        public bool DefaultMessageReceiver { get; set; }
        public int LastCalendarTimespan { get; set; }

        public LastPatientList LastPatientList { get; set; }
        public bool IsSupport => Id == 0;

        public ICollection<UserSiteHours> SiteHours { get; set; }
        public ICollection<UserGroupReference> UserReferences { get; set; }
    }
}
