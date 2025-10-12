using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain.Noah
{
    public class N4ManualTymp
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int TympId { get; set; }
        public string TympName { get; set; }
        public DateTime CreateDate { get; set; }
        public string TympData { get; set; }
        public bool Right { get; set; }
        public string TympDataUnits { get; set; }
        public int CanalVolume { get; set; }
        public string CanalVolumeUnits { get; set; }
        public int Gradient { get; set; }
        public string GradientUnits { get; set; }
        public int MaximumPressure { get; set; }
        public string MaximumPressureUnits { get; set; }
        public int ProbeSweepSpeed { get; set; }
        public int ProbeRecorderMode { get; set; }
        public int ProbeFrequency { get; set; }
        public int Pressure { get; set; }
        public int ResonanceFrequency { get; set; }
        public string Result { get; set; }
        public int Factor { get; set; }
        public byte[] PublicData { get; set; }
        public bool Void { get; set; }
    }
}
