using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IMarketingReferenceRepository
{
    Task<MarketingReference> Add(MarketingReference marketingReference);
    void Delete(int id);
    Task<MarketingReference> Get(int id);
    Task<List<MarketingReference>> GetAll(int marketingCategoryId, bool includeInactive);
    Task<MarketingReference> Update(MarketingReference marketingReference);
}

public class MarketingReferenceRepository : IMarketingReferenceRepository
{
    private readonly IMarketingReferenceUnitOfWork _marketingReferenceUnitOfWork;

    public MarketingReferenceRepository(IMarketingReferenceUnitOfWork marketingReferenceUnitOfWork)
    {
        _marketingReferenceUnitOfWork = marketingReferenceUnitOfWork;
    }

    public async Task<MarketingReference> Add(MarketingReference marketingReference)
    {
        return await _marketingReferenceUnitOfWork.Add(marketingReference);
    }

    public async Task<MarketingReference> Get(int id)
    {
        return await _marketingReferenceUnitOfWork.GetMarketingReference(id);
    }

	public async Task<List<MarketingReference>> GetAll(int marketingCategoryId, bool includeInactive)
	{
		return await _marketingReferenceUnitOfWork.GetMarketingReferences(x =>
			(includeInactive || !x.Inactive) && marketingCategoryId == x.CategoryId,
			null, x => x.Include(m => m.Sites));
	}

	public async Task<MarketingReference> Update(MarketingReference marketingReference)
    {
        return await _marketingReferenceUnitOfWork.Update(marketingReference);
    }

    public void Delete(int id)
    {
        _marketingReferenceUnitOfWork.Delete(id);
    }
}