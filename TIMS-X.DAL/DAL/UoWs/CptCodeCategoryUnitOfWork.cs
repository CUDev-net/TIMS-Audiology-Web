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

public interface ICptCodeCategoryUnitOfWork : IUnitOfWork
{
    Task<CptCodeCategory> GetCptCodeCategory(int id);

    Task<List<CptCodeCategory>> GetCptCodeCategories(Expression<Func<CptCodeCategory, bool>> filter = null,
        Func<IQueryable<CptCodeCategory>, IOrderedQueryable<CptCodeCategory>> orderBy = null,
        Func<IQueryable<CptCodeCategory>, IIncludableQueryable<CptCodeCategory, object>> includes = null);
}

public class CptCodeCategoryUnitOfWork : UnitOfWorkBase, ICptCodeCategoryUnitOfWork
{
    public CptCodeCategoryUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "CPTCodeCategory";

    public async Task<CptCodeCategory> GetCptCodeCategory(int id)
    {
        return await Single<CptCodeCategory>(u => u.Id == id);
    }

    public async Task<List<CptCodeCategory>> GetCptCodeCategories(Expression<Func<CptCodeCategory, bool>> filter = null,
        Func<IQueryable<CptCodeCategory>, IOrderedQueryable<CptCodeCategory>> orderBy = null,
        Func<IQueryable<CptCodeCategory>, IIncludableQueryable<CptCodeCategory, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}