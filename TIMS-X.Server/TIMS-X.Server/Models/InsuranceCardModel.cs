using TIMS_X.Core.Enums;

namespace TIMS_X.Server.Models;

public class InsuranceCardModel : PatientImageModel
{
	public bool Front { get; set; }
	public InsuranceSlot Slot { get; set; }
}