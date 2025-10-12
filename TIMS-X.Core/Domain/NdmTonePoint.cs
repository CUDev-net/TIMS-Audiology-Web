using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain
{
    public class NdmTonePoint
    {
        public long Id { get; set; }
        public long AudiogramId { get; set; }
        public int MaskingFrequency { get; set; }
        public int StimulusFrequency { get; set; }
        public int MaskingLevel { get; set; }
        public int StimulusLevel { get; set; }
        public int TonePointStatus { get; set; }

        public NdmAudiogram Audiogram { get; set; }
    }
}
