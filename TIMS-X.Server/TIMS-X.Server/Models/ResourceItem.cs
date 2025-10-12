using TIMS_X.Core.Domain;
using TIMS_X.Server.Utils;

namespace TIMS_X.Server.Models;

public class ResourceItem
{
	public ResourceItem(Resource resource)
	{
		Id = resource.Id;
		SiteId = resource.SiteId;
		Inactive = resource.Inactive;
		Name = resource.Name;
		Description = resource.Description;
		if (resource.Color.HasValue) Color = ColorHelper.ToRgbString(resource.Color.Value);
	}

	public string Color { get; set; }
	public string Description { get; set; }
	public int Id { get; set; }
	public bool Inactive { get; set; }
	public string Name { get; set; }
	public int SiteId { get; set; }
}