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

public interface IAppointmentTypeUnitOfWork : IUnitOfWork
{
    Task<AppointmentType> GetAppointmentType(int id);

    Task<List<AppointmentType>> GetAppointmentTypes(Expression<Func<AppointmentType, bool>> filter = null,
        Func<IQueryable<AppointmentType>, IOrderedQueryable<AppointmentType>> orderBy = null,
        Func<IQueryable<AppointmentType>, IIncludableQueryable<AppointmentType, object>> includes = null);
}

public class AppointmentTypeUnitOfWork : UnitOfWorkBase, IAppointmentTypeUnitOfWork
{
    public AppointmentTypeUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(AppointmentType);

    public async Task<AppointmentType> GetAppointmentType(int id)
    {
        return await Single<AppointmentType>(u => u.Id == id);
    }

    public async Task<List<AppointmentType>> GetAppointmentTypes(Expression<Func<AppointmentType, bool>> filter = null,
        Func<IQueryable<AppointmentType>, IOrderedQueryable<AppointmentType>> orderBy = null,
        Func<IQueryable<AppointmentType>, IIncludableQueryable<AppointmentType, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}