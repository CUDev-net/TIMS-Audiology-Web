using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class PatientRequiredField : Entity
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public bool Required { get; set; }
    }
}