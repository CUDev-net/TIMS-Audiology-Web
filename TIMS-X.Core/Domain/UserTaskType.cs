using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class UserTaskType : Entity
    {
        public string Name { get; set; }
        public bool Inactive { get; set; }
    }
}