using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class County : Entity
    {
        public bool Inactive { get; set; }
        public string Name { get; set; }
    }
}