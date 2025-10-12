using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain
{
    public class NdmMeasurementCondition
    {
        public long Id { get; set; }
        public int StimulusSignalType { get; set; }
        public int MaskingSignalType { get; set; }
        public int StimulusSignalOutput { get; set; }
        public int MaskingSignalOutput { get; set; }
        public int StimulusdBWeighting { get; set; }
        public int MaskingdBWeighting { get; set; }
        public int StimulusPresentationType { get; set; }
        public int MaskingPresentationType { get; set; }
        public int HearingInstrument1Condition { get; set; }
        public int HearingInstrument2Condition { get; set; }
    }
}
