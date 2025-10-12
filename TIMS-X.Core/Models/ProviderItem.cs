using System.Collections.Generic;
using System.Linq;
using TIMS_X.Core.Domain;

namespace TIMS_X.Core.Models
{
	public class ProviderItem
	{
		public ProviderItem()
		{
		}

		public ProviderItem(Provider provider)
		{
			Id = provider.Id;
			Inactive = provider.Inactive;
			Color = provider.Color;
			LastName = provider.LastName;
			FirstName = provider.FirstName;
			Initial = provider.Initial;

			SiteSchedules = provider.User?.SiteHours?
				.GroupBy(p => p.SiteId)
				.ToDictionary(g => g.Key, g => new WeeklySchedule(g.ToList()));
		}

		public Dictionary<int, WeeklySchedule> ApptTypeSchedules { get; set; }
		public int? Color { get; set; }
		public string FirstName { get; set; }

		public int Id { get; set; }
		public bool Inactive { get; set; }
		public string Initial { get; set; }

		public string LastFirstMiddle => LastName + ", " + FirstName +
		                                 (string.IsNullOrEmpty(Initial) ? string.Empty : " " + Initial + ".");

		public string LastName { get; set; }

		public Dictionary<int, WeeklySchedule> SiteSchedules { get; set; }

		public int UserId { get; set; }
	}
}