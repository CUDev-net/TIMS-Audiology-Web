using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.BLL.Utilities;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface ISiteRepository
{
	Task<Site> Add(Site site);
	void Delete(int id);
	Task<Site> Get(int id);
	Task<List<Site>> GetSummaries(bool includeInactive);
	Task<List<Site>> GetForPatientScheduling();
	Task<Site> GetSummary(int id);
	Task<Site> Update(Site site);
}

public class SiteRepository : ISiteRepository
{
	private readonly ISiteUnitOfWork _siteUnitOfWork;

	public SiteRepository(ISiteUnitOfWork siteUnitOfWork)
	{
		_siteUnitOfWork = siteUnitOfWork;
	}

	public Task<Site> Add(Site site)
	{
		return _siteUnitOfWork.Add(site);
	}

	public Task<Site> Get(int id)
	{
		return _siteUnitOfWork.GetSite(id);
	}

	public async Task<List<Site>> GetSummaries(bool includeInactive)
	{
		var sites = await _siteUnitOfWork.GetSiteSummaries(s => includeInactive || !s.Inactive);
		_TranslateColors(sites);
		return sites;
	}

	public async Task<List<Site>> GetForPatientScheduling()
	{
		return await _siteUnitOfWork.GetSiteSummaries(s => s.UseForPatientScheduling && !s.Inactive);
	}

	public async Task<Site> GetSummary(int id)
	{
		var sites = await _siteUnitOfWork.GetSiteSummaries(s => s.Id == id);
		_TranslateColors(sites);
		return sites.FirstOrDefault();
	}

	public async Task<Site> Update(Site site)
	{
		return await _siteUnitOfWork.Update(site);
	}

	public void Delete(int id)
	{
		_siteUnitOfWork.Delete(id);
	}

	private void _TranslateColors(List<Site> sites)
	{
		foreach (var site in sites)
			site.WebColor = ColorHelper.GetHexColor(site.Color);
	}
}