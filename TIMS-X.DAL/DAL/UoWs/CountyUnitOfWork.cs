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

public interface ICountyUnitOfWork : IUnitOfWork
{
    Task<County> GetCounty(int id);

    Task<List<County>> GetCounties(Expression<Func<County, bool>> filter = null,
        Func<IQueryable<County>, IOrderedQueryable<County>> orderBy = null,
        Func<IQueryable<County>, IIncludableQueryable<County, object>> includes = null);
}

public class CountyUnitOfWork : UnitOfWorkBase, ICountyUnitOfWork
{
    public CountyUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(County);

    public async Task<County> GetCounty(int id)
    {
        return await Single<County>(u => u.Id == id);
    }

    public async Task<List<County>> GetCounties(Expression<Func<County, bool>> filter = null,
        Func<IQueryable<County>, IOrderedQueryable<County>> orderBy = null,
        Func<IQueryable<County>, IIncludableQueryable<County, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}