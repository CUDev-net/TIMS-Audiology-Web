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

public interface IHaStockItemStatusUnitOfWork : IUnitOfWork
{
    Task<HaStockItemStatus> GetHaStockItemStatus(int id);

    Task<List<HaStockItemStatus>> GetHaStockItemStatuses(
        Expression<Func<HaStockItemStatus, bool>> filter = null,
        Func<IQueryable<HaStockItemStatus>, IOrderedQueryable<HaStockItemStatus>> orderBy = null,
        Func<IQueryable<HaStockItemStatus>, IIncludableQueryable<HaStockItemStatus, object>> includes = null);
}

public class HaStockItemStatusUnitOfWork : UnitOfWorkBase, IHaStockItemStatusUnitOfWork
{
    public HaStockItemStatusUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "HAStockItemStatus";

    public async Task<HaStockItemStatus> GetHaStockItemStatus(int id)
    {
        return await Single<HaStockItemStatus>(u => u.Id == id);
    }

    public async Task<List<HaStockItemStatus>> GetHaStockItemStatuses(
        Expression<Func<HaStockItemStatus, bool>> filter = null,
        Func<IQueryable<HaStockItemStatus>, IOrderedQueryable<HaStockItemStatus>> orderBy = null,
        Func<IQueryable<HaStockItemStatus>, IIncludableQueryable<HaStockItemStatus, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}