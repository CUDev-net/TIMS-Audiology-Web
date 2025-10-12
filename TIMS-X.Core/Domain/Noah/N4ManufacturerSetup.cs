namespace TIMS_X.Core.Domain.Noah
{
    public class N4ManufacturerSetup
    {
        public int Id { get; set; }
        public int ManufacturerId { get; set; }
        public string Key { get; set; }
        public byte[] SetupData { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
