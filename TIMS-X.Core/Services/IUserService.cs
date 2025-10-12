using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Models;

namespace TIMS_X.Core.Services
{
	public interface IUserService
	{
		Task<int> GetFirstUserIdAsync();
		Task<IEnumerable<UserItem>> GetLoginUsersAsync(bool includeInactive, string officeCode);
		Task<List<PatientItem>> GetRecentPatientsListAsync(int userId);
		Task<bool> LoginAsync(string officeCode, int siteId, UserItem userItem, string password);
		Task PutRecentPatientAsync(int userId, int patientId);
	}
}