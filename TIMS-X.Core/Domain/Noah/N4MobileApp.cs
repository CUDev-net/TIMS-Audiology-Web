namespace TIMS_X.Core.Domain.Noah
{
    public class N4MobileApp
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public int MobileAppType { get; set; }
        public int AcceptState { get; set; }
        public string ActiveRequirements { get; set; }
        public string PendingRequirements { get; set; }
    }
}
