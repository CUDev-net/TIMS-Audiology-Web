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

public interface IHaHistoryUnitOfWork : IUnitOfWork
{
    Task<HaHistory> GetHaHistory(int id,
        Func<IQueryable<HaHistory>, IIncludableQueryable<HaHistory, object>> includes = null);

    Task<List<HaHistory>> GetHaHistories(Expression<Func<HaHistory, bool>> filter = null,
        Func<IQueryable<HaHistory>, IOrderedQueryable<HaHistory>> orderBy = null,
        Func<IQueryable<HaHistory>, IIncludableQueryable<HaHistory, object>> includes = null);
}

public class HaHistoryUnitOfWork : UnitOfWorkBase, IHaHistoryUnitOfWork
{
    public HaHistoryUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "HAHistory";

    public async Task<HaHistory> GetHaHistory(int id,
        Func<IQueryable<HaHistory>, IIncludableQueryable<HaHistory, object>> includes = null)
    {
        return await Single(h => h.Id == id, includes);
    }


    public async Task<List<HaHistory>> GetHaHistories(Expression<Func<HaHistory, bool>> filter = null,
        Func<IQueryable<HaHistory>, IOrderedQueryable<HaHistory>> orderBy = null,
        Func<IQueryable<HaHistory>, IIncludableQueryable<HaHistory, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}