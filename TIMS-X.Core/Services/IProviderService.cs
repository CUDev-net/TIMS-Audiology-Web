using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Models;

namespace TIMS_X.Core.Services
{
	public interface IProviderService
	{
		Task<IEnumerable<ProviderItem>> GetProvidersAsync(bool includeInactive);
	}
}