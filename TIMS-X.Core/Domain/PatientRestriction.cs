using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class PatientRestriction : Entity
    {
        public int? PatientId { get; set; }
        public Patient Patient { get; set; }
        public int RestrictionId { get; set; }
        public CommunicationRestriction CommunicationRestriction { get; set; }
    }
}
