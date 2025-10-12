
namespace TIMS_X.Core.Models
{
    public class UserItem
    {
        public bool UsingActiveDirectory { get; set; }
        public int Id { get; set; }
        public bool Inactive { get; set; }
        public string Name { get; set; }
        public string AdDomain { get; set; }
        public string AdUsername { get; set; }
        public bool IsWebUser { get; set; }
        public int SiteId { get; set; }
        public string DisplayName => UsingActiveDirectory ?
            $"{AdDomain}\\{AdUsername}" : Name;
    }
}
