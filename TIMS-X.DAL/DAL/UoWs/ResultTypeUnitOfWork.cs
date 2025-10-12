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

public interface IResultTypeUnitOfWork : IUnitOfWork
{
    Task<ResultType> GetResultType(int id);

    Task<List<ResultType>> GetResultTypes(Expression<Func<ResultType, bool>> filter = null,
        Func<IQueryable<ResultType>, IOrderedQueryable<ResultType>> orderBy = null,
        Func<IQueryable<ResultType>, IIncludableQueryable<ResultType, object>> includes = null);
}

public class ResultTypeUnitOfWork : UnitOfWorkBase, IResultTypeUnitOfWork
{
    public ResultTypeUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(ResultType);

    public async Task<ResultType> GetResultType(int id)
    {
        return await Single<ResultType>(u => u.Id == id);
    }

    public async Task<List<ResultType>> GetResultTypes(Expression<Func<ResultType, bool>> filter = null,
        Func<IQueryable<ResultType>, IOrderedQueryable<ResultType>> orderBy = null,
        Func<IQueryable<ResultType>, IIncludableQueryable<ResultType, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}