using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class UserGroupReference : Entity
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public UserGroup UserGroup { get; set; }
    }
}