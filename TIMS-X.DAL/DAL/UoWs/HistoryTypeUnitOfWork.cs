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

public interface IHistoryTypeUnitOfWork : IUnitOfWork
{
    Task<HistoryType> GetHistoryType(int id);

    Task<List<HistoryType>> GetHistoryTypes(Expression<Func<HistoryType, bool>> filter = null,
        Func<IQueryable<HistoryType>, IOrderedQueryable<HistoryType>> orderBy = null,
        Func<IQueryable<HistoryType>, IIncludableQueryable<HistoryType, object>> includes = null);
}

public class HistoryTypeUnitOfWork : UnitOfWorkBase, IHistoryTypeUnitOfWork
{
    public HistoryTypeUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(HistoryType);

    public async Task<HistoryType> GetHistoryType(int id)
    {
        return await Single<HistoryType>(u => u.Id == id);
    }

    public async Task<List<HistoryType>> GetHistoryTypes(Expression<Func<HistoryType, bool>> filter = null,
        Func<IQueryable<HistoryType>, IOrderedQueryable<HistoryType>> orderBy = null,
        Func<IQueryable<HistoryType>, IIncludableQueryable<HistoryType, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}