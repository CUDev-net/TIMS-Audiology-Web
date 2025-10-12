namespace TIMS_X.NoahDataInterface413.Models
{
	public enum Noah4SystemCommandEnum
	{
		ValidateUser = 0,
		IsLoginSystemEnabled = 1,
		GetUsers = 2,
		GetUser = 3,
		DeleteUser = 4,
		PutUser = 5,
		GetRoles = 6,
		GetDefaultRoles = 7,
		GetRole = 8,
		GetRoleFromUserID = 9,
		PutRole = 10,
		GetPatients = 11,
		GetPatientsCount = 12,
		GetPatient = 13,
		PutPatient = 14,
		DeletePatient = 15,
		GetPatientComment = 16,
		PutPatientComment = 17,
		GetDatabaseProperties = 18,
		SetLoginSystemInf = 19,
		GetLoginSystemInf = 20,
		SendEventPatientUpdated = 21
	}

	public class Noah4SystemCommand
	{
		public object Data;

		public Noah4SystemCommandEnum Command { get; set; }
	}
}