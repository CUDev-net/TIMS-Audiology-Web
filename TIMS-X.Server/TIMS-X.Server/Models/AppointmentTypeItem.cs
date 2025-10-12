using TIMS_X.Core.Domain;
using TIMS_X.Server.Utils;

namespace TIMS_X.Server.Models;

public class AppointmentTypeItem
{
	public AppointmentTypeItem(AppointmentType appointmentType)
	{
		Id = appointmentType.Id;
		Inactive = appointmentType.Inactive;
		Name = appointmentType.Name;
		Description = appointmentType.Description;
		if (appointmentType.Color.HasValue)
		{
			BackgroundColor = ColorHelper.ToRgbString(appointmentType.Color.Value);
			ForegroundColor = ColorHelper.GetForegroundRgbString(appointmentType.Color.Value);
		}
		else
		{
			BackgroundColor = "RGB(255,255,255)";
			ForegroundColor = "RGB(0,0,0)";
		}
	}

	public string BackgroundColor { get; set; }
	public string Description { get; set; }
	public string ForegroundColor { get; set; }
	public int Id { get; set; }
	public bool Inactive { get; set; }
	public string Name { get; set; }
}