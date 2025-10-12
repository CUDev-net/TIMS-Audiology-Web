using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class LastPatientList : Entity
    {
        public int UserId { get; set; }
        public string PatientListXml { get; set; }
    }
}
