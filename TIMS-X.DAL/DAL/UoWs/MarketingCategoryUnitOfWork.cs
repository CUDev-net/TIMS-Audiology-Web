using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IMarketingCategoryUnitOfWork : IUnitOfWork
{
    Task<MarketingReferenceCategory> GetMarketingCategory(int id);

    Task<List<MarketingReferenceCategory>> GetMarketingCategories(
        Expression<Func<MarketingReferenceCategory, bool>> filter = null,
        Func<IQueryable<MarketingReferenceCategory>, IOrderedQueryable<MarketingReferenceCategory>> orderBy = null,
        Func<IQueryable<MarketingReferenceCategory>, IIncludableQueryable<MarketingReferenceCategory, object>>
            includes = null);
}

public class MarketingCategoryUnitOfWork : UnitOfWorkBase, IMarketingCategoryUnitOfWork
{
    public MarketingCategoryUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(
        context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "MktCategory";

    public async Task<MarketingReferenceCategory> GetMarketingCategory(int id)
    {
        return await Single<MarketingReferenceCategory>(u => u.Id == id);
    }

    public async Task<List<MarketingReferenceCategory>> GetMarketingCategories(
        Expression<Func<MarketingReferenceCategory, bool>> filter = null,
        Func<IQueryable<MarketingReferenceCategory>, IOrderedQueryable<MarketingReferenceCategory>> orderBy = null,
        Func<IQueryable<MarketingReferenceCategory>, IIncludableQueryable<MarketingReferenceCategory, object>>
            includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}