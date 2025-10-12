using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class HaModelOption : Entity
    {
        public int? HaOptionId { get; set; }
        public int? HaModelId { get; set; }
        public int? UpdatedUserId { get; set; }
        public HaModel HaModel { get; set; }
    }
}