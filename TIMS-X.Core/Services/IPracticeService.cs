using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;

namespace TIMS_X.Core.Services
{
	public interface IPracticeService
	{
		Task<string> GetPracticeNameAsync(string officeCode);
		Task<string> GetServerVersionAsync();
		Task<IEnumerable<SiteItem>> GetSitesAsync(bool includeInactive, string officeCode);
		Task<ConnectionTestResult> TestConnectionAsync(string officeCode);
		Task<bool> UsesAdAuthenticationAsync(string officeCode);
	}
}