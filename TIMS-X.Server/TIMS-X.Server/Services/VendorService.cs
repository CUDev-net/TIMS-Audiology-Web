using System.Threading.Tasks;
using TIMS_X.Core.Models;
using TIMS_X.Server.Queries;

namespace TIMS_X.Server.Services;

public class VendorService
{
	private readonly PracticeQuery _practiceQuery;

	public VendorService(PracticeQuery practiceQuery)
	{
		_practiceQuery = practiceQuery;
	}

	public async Task<PracticeItem> GetPracticeAsync()
	{
		var practice = await _practiceQuery.GetPracticeAsync();
		var sites = await _practiceQuery.GetSitesAsync(false);

		return new PracticeItem(practice, sites);
	}
}