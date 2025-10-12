using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IMarketingCategoryRepository
{
    Task<MarketingReferenceCategory> Add(MarketingReferenceCategory marketingCategory);
    Task<MarketingReferenceCategory> Get(int id);
    Task<List<MarketingReferenceCategory>> GetAll(bool includeInactive);
    Task<MarketingReferenceCategory> Update(MarketingReferenceCategory marketingCategory);
    void Delete(int id);
}

public class MarketingCategoryRepository : IMarketingCategoryRepository
{
    private readonly IMarketingCategoryUnitOfWork _marketingCategoryUnitOfWork;

    public MarketingCategoryRepository(IMarketingCategoryUnitOfWork marketingCategoryUnitOfWork)
    {
        _marketingCategoryUnitOfWork = marketingCategoryUnitOfWork;
    }

    public async Task<MarketingReferenceCategory> Add(MarketingReferenceCategory marketingCategory)
    {
        return await _marketingCategoryUnitOfWork.Add(marketingCategory);
    }

    public async Task<MarketingReferenceCategory> Get(int id)
    {
        return await _marketingCategoryUnitOfWork.GetMarketingCategory(id);
    }

    public async Task<List<MarketingReferenceCategory>> GetAll(bool includeInactive)
    {
        return await _marketingCategoryUnitOfWork.GetMarketingCategories(
            x => includeInactive || !x.Inactive,
            null,
            i => i.Include(x => x.MarketingReferences)
	            .ThenInclude( s => s.Sites));
    }

    public async Task<MarketingReferenceCategory> Update(MarketingReferenceCategory marketingCategory)
    {
        return await _marketingCategoryUnitOfWork.Update(marketingCategory);
    }

    public void Delete(int id)
    {
        _marketingCategoryUnitOfWork.Delete(id);
    }
}