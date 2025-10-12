using System.Collections.Generic;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class NdmAudiogram
    {
        public NdmAudiogram()
        {
            TonePoints = new HashSet<NdmTonePoint>();
        }

        public long Id { get; set; }
        public long ActionId { get; set; }
        public long MeasurementConditionId { get; set; }
        public int AudiogramType { get; set; }
        public int PatientId { get; set; }
        public int Side { get; set; }

        public NdmMeasurementCondition MeasurementCondition { get; set; }
        public NdmAction Action { get; set; }
        public ICollection<NdmTonePoint> TonePoints { get; set; }
    }
}
