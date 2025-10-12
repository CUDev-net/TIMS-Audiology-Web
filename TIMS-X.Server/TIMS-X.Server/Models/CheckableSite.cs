using System.Collections.Generic;
using System.Linq;
using TIMS_X.Core.Models;

namespace TIMS_X.Server.Models;

public class CheckableSite : SiteItem
{
	public CheckableSite(SiteItem site)
	{
		Id = site.Id;
		Inactive = site.Inactive;
		Color = site.Color;
		Name = site.Name;
		Address1 = site.Address1;
		Address2 = site.Address2;
		City = site.City;
		State = site.State;
		ZipCode = site.ZipCode;
		WeeklySchedule = site.WeeklySchedule;
		Resources = site.Resources;
		CheckableResources = Resources.Select(x => new CheckedItem<ResourceItem>(new ResourceItem(x))).ToList();
	}

	public List<CheckedItem<ResourceItem>> CheckableResources { get; set; }
	public bool IsChecked { get; set; }
}