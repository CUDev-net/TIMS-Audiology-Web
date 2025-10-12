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

public interface IResultUnitOfWork : IUnitOfWork
{
    Task<Result> GetResult(int id);

    Task<List<Result>> GetResults(Expression<Func<Result, bool>> filter = null,
        Func<IQueryable<Result>, IOrderedQueryable<Result>> orderBy = null,
        Func<IQueryable<Result>, IIncludableQueryable<Result, object>> includes = null);
}

public class ResultUnitOfWork : UnitOfWorkBase, IResultUnitOfWork
{
    public ResultUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(Result);

    public async Task<Result> GetResult(int id)
    {
        return await Single<Result>(u => u.Id == id);
    }

    public async Task<List<Result>> GetResults(Expression<Func<Result, bool>> filter = null,
        Func<IQueryable<Result>, IOrderedQueryable<Result>> orderBy = null,
        Func<IQueryable<Result>, IIncludableQueryable<Result, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}