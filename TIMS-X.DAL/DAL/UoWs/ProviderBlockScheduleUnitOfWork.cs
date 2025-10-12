using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IProviderBlockScheduleUnitOfWork : IUnitOfWork
{
    IQueryable<ProviderBlockSchedule> GetProviderBlockSchedules(
        Expression<Func<ProviderBlockSchedule, bool>> filter = null,
        Func<IQueryable<ProviderBlockSchedule>, IOrderedQueryable<ProviderBlockSchedule>> orderBy = null,
        Func<IQueryable<ProviderBlockSchedule>, IIncludableQueryable<ProviderBlockSchedule, object>> includes = null);
}

public class ProviderBlockScheduleUnitOfWork : UnitOfWorkBase, IProviderBlockScheduleUnitOfWork
{
    public ProviderBlockScheduleUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(
        context, httpContextAccessor)
    {
    }

    protected override string TableName => "ProviderBlockReference";

    public IQueryable<ProviderBlockSchedule> GetProviderBlockSchedules(
        Expression<Func<ProviderBlockSchedule, bool>> filter = null,
        Func<IQueryable<ProviderBlockSchedule>, IOrderedQueryable<ProviderBlockSchedule>> orderBy = null,
        Func<IQueryable<ProviderBlockSchedule>, IIncludableQueryable<ProviderBlockSchedule, object>> includes = null)
    {
        return Get(filter, orderBy, includes);
    }
}