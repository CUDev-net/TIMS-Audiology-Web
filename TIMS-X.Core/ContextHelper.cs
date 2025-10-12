using TIMS_X.Core.Domain;

namespace TIMS_X.Core
{
    public class ContextHelper
    {
        public bool UseActiveDirectoryAuthentication { get; set; }
        public User CurrentUser { get; set; }
        public Site CurrentSite { get; set; }
        public string OfficeCode { get; set; }
        public string SignalrConnectionName => OfficeCode.ToLower();
    }
}