using System;
using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class UserGroup : Entity, IUpdateAudited
    {
        public UserGroup()
        {
            UserReferences = new HashSet<UserGroupReference>();
        }

        public string Name { get; set; }
        public bool Inactive { get; set; }
        public bool Protected { get; set; }
        public string Description { get; set; }

        public ICollection<UserGroupAppSetting> Settings { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public ICollection<UserGroupReference> UserReferences { get; set; }
    }
}