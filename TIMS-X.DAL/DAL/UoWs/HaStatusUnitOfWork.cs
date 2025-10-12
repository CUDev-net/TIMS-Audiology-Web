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

public interface IHaStatusUnitOfWork : IUnitOfWork
{
    Task<HaStatus> GetHaStatus(int id);

    Task<List<HaStatus>> GetHaStatuses(
        Expression<Func<HaStatus, bool>> filter = null,
        Func<IQueryable<HaStatus>, IOrderedQueryable<HaStatus>> orderBy = null,
        Func<IQueryable<HaStatus>, IIncludableQueryable<HaStatus, object>> includes = null);
}

public class HaStatusUnitOfWork : UnitOfWorkBase, IHaStatusUnitOfWork
{
    public HaStatusUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "HAStatus";

    public async Task<HaStatus> GetHaStatus(int id)
    {
        return await Single<HaStatus>(u => u.Id == id);
    }

    public async Task<List<HaStatus>> GetHaStatuses(
        Expression<Func<HaStatus, bool>> filter = null,
        Func<IQueryable<HaStatus>, IOrderedQueryable<HaStatus>> orderBy = null,
        Func<IQueryable<HaStatus>, IIncludableQueryable<HaStatus, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}