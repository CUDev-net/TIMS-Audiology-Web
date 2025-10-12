namespace TIMS_X.Core.Domain.Noah
{
    public class N4PatientSetup
    {
        public int PatientId { get; set; }
        public int ModuleId { get; set; }
        public byte[] SetupData { get; set; }

        public Patient Patient { get; set; }
    }
}
