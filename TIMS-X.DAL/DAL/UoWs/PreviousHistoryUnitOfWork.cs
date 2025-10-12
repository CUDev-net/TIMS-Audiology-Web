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

public interface IPreviousHistoryUnitOfWork : IUnitOfWork
{
    Task<PreviousHistory> GetPreviousHistory(int id);

    Task<List<PreviousHistory>> GetPreviousHistories(Expression<Func<PreviousHistory, bool>> filter = null,
        Func<IQueryable<PreviousHistory>, IOrderedQueryable<PreviousHistory>> orderBy = null,
        Func<IQueryable<PreviousHistory>, IIncludableQueryable<PreviousHistory, object>> includes = null);
}

public class PreviousHistoryUnitOfWork : UnitOfWorkBase, IPreviousHistoryUnitOfWork
{
    public PreviousHistoryUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(PreviousHistory);

    public async Task<PreviousHistory> GetPreviousHistory(int id)
    {
        return await Single<PreviousHistory>(u => u.Id == id);
    }

    public async Task<List<PreviousHistory>> GetPreviousHistories(Expression<Func<PreviousHistory, bool>> filter = null,
        Func<IQueryable<PreviousHistory>, IOrderedQueryable<PreviousHistory>> orderBy = null,
        Func<IQueryable<PreviousHistory>, IIncludableQueryable<PreviousHistory, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}