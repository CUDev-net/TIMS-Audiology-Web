using System;
using TIMS_X.Core.Attributes;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class Schedule : Entity, ICreateByUserAudited, ICreateDateAudited, IUpdateAudited, IRecurrable, ICloneable
    {
        public byte[] RowVersion { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public int? RecurringIntervalId { get; set; }
        public int SiteId { get; set; }
        public int[] SiteIds { get; set; }
        public int ProviderId { get; set; }
        public int[] ProviderIds { get; set; }
        public int? Color { get; set; }
        public int UpdatedSiteId { get; set; }
        public string Web_Color { get; set; }
        public string UpdatedByUserName { get; set; }

        [TimsObject] public RecurringInterval RecurringInterval { get; set; }

        public object Clone()
        {
            return new Schedule
            {
                StartsAt = StartsAt,
                EndsAt = EndsAt,
                Title = Title,
                Location = Location,
                Notes = Notes,
                RecurringIntervalId = RecurringIntervalId,
                SiteId = SiteId,
                SiteIds = SiteIds,
                ProviderId = ProviderId,
                ProviderIds = ProviderIds,
                Color = Color,
                UpdatedUserId = UpdatedUserId,
                UpdatedSiteId = UpdatedSiteId,
                UpdatedDate = UpdatedDate,
                CreatedUserId = CreatedUserId,
                CreatedDate = CreatedDate,
                Web_Color = Web_Color,
                UpdatedByUserName = UpdatedByUserName,
                RecurringInterval = RecurringInterval?.Clone() as RecurringInterval
            };
        }

        public int? CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}