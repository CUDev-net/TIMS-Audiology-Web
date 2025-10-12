using System.Collections.Generic;
using TIMS_X.Core.Attributes;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class ProviderSummary : Entity
    {
        public bool Inactive { get; set; }
        public string LastName { get; set; }
        public bool UsePracticeIds { get; set; }
        public string FirstName { get; set; }
        public string Initial { get; set; }
        public string Degree { get; set; }
        public bool Deleted { get; set; }
        public int? Color { get; set; }
        public string Npi { get; set; }
        public int DisplayOrder { get; set; }
        public int UserId { get; set; }
        [TimsObject] public User User { get; set; }

        public string SimpleName =>
            $"{FirstName} {LastName}";

        public string FullName =>
            $"{LastName}, {FirstName}{(string.IsNullOrWhiteSpace(Initial) ? string.Empty : " " + Initial + ".")}";

        public List<HoursOfOperationModel> Hours { get; set; }
        public List<UserSiteHours> SiteHours { get; set; }
        public string WebColor { get; set; }
    }
}