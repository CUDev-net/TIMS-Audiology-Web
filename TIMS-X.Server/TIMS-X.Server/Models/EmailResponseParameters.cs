using TIMS_X.Core.Enums;

namespace TIMS_X.Server.Models;

public class EmailResponseParameters
{
	public int LogId { get; set; }
	public PatientNotificationResponse ResponseCode { get; set; }
}