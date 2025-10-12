using TIMS_X.Core.Domain.Base;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Domain
{
    public class UserGroupAppSetting : Entity
    {
        public int GroupId { get; set; }
        public SettingEnum PermissionType { get; set; }
    }
}
