using TIMS_X.Core.Enums;

namespace TIMS_X.Server.Models;

public class PatientFormPayload
{
	public PatientFormTypeEnum FormType { get; set; }
	public string OfficeCode { get; set; }

	public int PatientId { get; set; }
	public string TimsToken { get; set; }
	public int UserId { get; set; }
}