using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class PreviousHistory : Entity
    {
        public bool Protected { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public int ConditionId { get; set; }
        public MedicalCondition Condition { get; set; }
    }
}
