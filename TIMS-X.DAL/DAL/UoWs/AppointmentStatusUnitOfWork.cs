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

public interface IAppointmentStatusUnitOfWork : IUnitOfWork
{
    Task<AppointmentStatus> GetAppointmentStatus(int id);

    Task<List<AppointmentStatus>> GetAppointmentStatuses(Expression<Func<AppointmentStatus, bool>> filter = null,
        Func<IQueryable<AppointmentStatus>, IOrderedQueryable<AppointmentStatus>> orderBy = null,
        Func<IQueryable<AppointmentStatus>, IIncludableQueryable<AppointmentStatus, object>> includes = null);
}

public class AppointmentStatusUnitOfWork : UnitOfWorkBase, IAppointmentStatusUnitOfWork
{
    public AppointmentStatusUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(AppointmentStatus);

    public async Task<AppointmentStatus> GetAppointmentStatus(int id)
    {
        return await Single<AppointmentStatus>(u => u.Id == id);
    }

    public async Task<List<AppointmentStatus>> GetAppointmentStatuses(Expression<Func<AppointmentStatus, bool>> filter = null,
        Func<IQueryable<AppointmentStatus>, IOrderedQueryable<AppointmentStatus>> orderBy = null,
        Func<IQueryable<AppointmentStatus>, IIncludableQueryable<AppointmentStatus, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}