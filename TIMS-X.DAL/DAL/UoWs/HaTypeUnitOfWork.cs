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

public interface IHaTypeUnitOfWork : IUnitOfWork
{
    Task<HaType> GetHaType(int id);

    Task<List<HaType>> GetHaTypes(
        Expression<Func<HaType, bool>> filter = null,
        Func<IQueryable<HaType>, IOrderedQueryable<HaType>> orderBy = null,
        Func<IQueryable<HaType>, IIncludableQueryable<HaType, object>> includes = null);
}

public class HaTypeUnitOfWork : UnitOfWorkBase, IHaTypeUnitOfWork
{
    public HaTypeUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "HAType";

    public async Task<HaType> GetHaType(int id)
    {
        return await Single<HaType>(u => u.Id == id);
    }

    public async Task<List<HaType>> GetHaTypes(
        Expression<Func<HaType, bool>> filter = null,
        Func<IQueryable<HaType>, IOrderedQueryable<HaType>> orderBy = null,
        Func<IQueryable<HaType>, IIncludableQueryable<HaType, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}