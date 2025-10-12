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

public interface IKpiSiteTargetUnitOfWork : IUnitOfWork
{
    Task<KpiSiteTarget> GetKpiSiteTarget(int id);

    Task<List<KpiSiteTarget>> GetKpiSiteTargets(Expression<Func<KpiSiteTarget, bool>> filter = null,
        Func<IQueryable<KpiSiteTarget>, IOrderedQueryable<KpiSiteTarget>> orderBy = null,
        Func<IQueryable<KpiSiteTarget>, IIncludableQueryable<KpiSiteTarget, object>> includes = null);
}

public class KpiSiteTargetUnitOfWork : UnitOfWorkBase, IKpiSiteTargetUnitOfWork
{
    public KpiSiteTargetUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "KPISiteTarget";

    public async Task<KpiSiteTarget> GetKpiSiteTarget(int id)
    {
        return await Single<KpiSiteTarget>(u => u.Id == id);
    }

    public async Task<List<KpiSiteTarget>> GetKpiSiteTargets(Expression<Func<KpiSiteTarget, bool>> filter = null,
        Func<IQueryable<KpiSiteTarget>, IOrderedQueryable<KpiSiteTarget>> orderBy = null,
        Func<IQueryable<KpiSiteTarget>, IIncludableQueryable<KpiSiteTarget, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}