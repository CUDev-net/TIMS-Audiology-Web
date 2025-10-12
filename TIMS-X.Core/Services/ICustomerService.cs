using System.Threading.Tasks;
using TIMS_X.Core.Models;

namespace TIMS_X.Core.Services
{
	public interface ICustomerService
	{
		Task<ConnectionInfo> GetConnectionInfoAsync(string officeCode);
		Task<bool> PingAsync(string serverUrl, string port);
		Task<bool> ValidateCustomerAsync(string officeCode, string server, string port);
	}
}