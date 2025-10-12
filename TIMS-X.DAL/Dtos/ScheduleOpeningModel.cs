using System;

namespace TIMS_X.DAL.Dtos
{
    [Serializable]
    public class ScheduleOpeningModel
    {
        public DateTime EndsAt { get; set; }

        public string ProviderFirstName { get; set; }

        public int ProviderId { get; set; }

        public string ProviderLastName { get; set; }

        public string ProviderMiddleInitial { get; set; }

        public int? ResourceId { get; set; }

        public string ResourceName { get; set; }

        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public DateTime StartsAt { get; set; }

        public int DurationTotalMinutes => (int)Math.Ceiling(EndsAt.Subtract(StartsAt).TotalMinutes);
    }
}