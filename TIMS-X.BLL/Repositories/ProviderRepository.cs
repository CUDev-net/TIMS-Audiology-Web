using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.BLL.Utilities;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IProviderRepository
{
	Task<Provider> Add(Provider provider);
	Task<Provider> Get(int id);
	Task<List<ProviderSummary>> GetSummaries(bool includeInactive);
	Task<List<ProviderSummary>> GetSummariesForPatientScheduling();
	Task<ProviderSummary> GetSummary(int id);
	Task<List<ProviderSummary>> GetWithHours(List<int> providerIds = null);
	Task<Provider> Update(Provider provider);
}

public class ProviderRepository : IProviderRepository
{
	private readonly IProvidersUnitOfWork _providersUnitOfWork;
	private readonly ITimsUserSiteUnitOfWork _timsUserSiteUnitOfWork;

	public ProviderRepository(IProvidersUnitOfWork providersUnitOfWork, ITimsUserSiteUnitOfWork timsUserSiteUnitOfWork)
	{
		_providersUnitOfWork = providersUnitOfWork;
		_timsUserSiteUnitOfWork = timsUserSiteUnitOfWork;
	}

	public async Task<Provider> Add(Provider provider)
	{
		return await _providersUnitOfWork.Add(provider);
	}

	public async Task<Provider> Get(int id)
	{
		return await _providersUnitOfWork.GetProvider(id);
	}

	public async Task<ProviderSummary> GetSummary(int id)
	{
		return await _providersUnitOfWork.GetProviderSummaries(x => x.Id == id).FirstOrDefaultAsync();
	}

	public async Task<List<ProviderSummary>> GetSummaries(bool includeInactive)
	{
		return await _providersUnitOfWork.GetProviderSummaries(x => (includeInactive || !x.Inactive) && !x.Deleted)
			.ToListAsync();
	}

	public async Task<List<ProviderSummary>> GetSummariesForPatientScheduling()
	{
		var providers = new List<ProviderSummary> { new() { Id = -1, FirstName = "<any", LastName = "provider>" } };

		providers.AddRange(await _providersUnitOfWork
			.GetProviderSummaries(x => !x.Inactive && !x.Deleted && x.UseForPatientScheduling)
			.ToListAsync());
		return providers;
	}

	public async Task<List<ProviderSummary>> GetWithHours(List<int> providerIds = null)
	{
		var rawProviders = _providersUnitOfWork.GetProviderSummaries(x => !x.Inactive && !x.Deleted);
		if (providerIds != null)
			rawProviders = rawProviders.Where(p => providerIds.Contains(p.Id));
		var providers = await rawProviders.ToListAsync();
		_TranslateColors(providers);
		var siteHours = _timsUserSiteUnitOfWork.GetUserSites(
				providers.Select(p => p.UserId).ToList(), null, x => x.Include(s => s.Site))
			.ToLookup(l => l.UserId);
		var providerHours = await _providersUnitOfWork.GetProviderHours();
		foreach (var provider in providers)
			if (providerHours.ContainsKey(provider.Id))
			{
				provider.SiteHours = siteHours[provider.UserId].ToList();
				provider.Hours = providerHours[provider.Id].ToList();
			}
			else
			{
				provider.Hours = new List<HoursOfOperationModel>();
				provider.SiteHours = new List<UserSiteHours>();
			}

		return providers;
	}

	public Task<Provider> Update(Provider provider)
	{
		return _providersUnitOfWork.Update(provider);
	}

	private void _TranslateColors(List<ProviderSummary> providers)
	{
		foreach (var providerSummary in providers)
			providerSummary.WebColor = ColorHelper.GetHexColor(providerSummary.Color);
	}
}