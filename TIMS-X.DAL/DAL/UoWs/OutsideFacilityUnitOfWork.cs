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

public interface IOutsideFacilityUnitOfWork : IUnitOfWork
{
    Task<OutsideFacility> GetOutsideFacility(int id);

    Task<List<OutsideFacility>> GetOutsideFacilities(Expression<Func<OutsideFacility, bool>> filter = null,
        Func<IQueryable<OutsideFacility>, IOrderedQueryable<OutsideFacility>> orderBy = null,
        Func<IQueryable<OutsideFacility>, IIncludableQueryable<OutsideFacility, object>> includes = null);
}

public class OutsideFacilityUnitOfWork : UnitOfWorkBase, IOutsideFacilityUnitOfWork
{
    public OutsideFacilityUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(OutsideFacility);

    public async Task<OutsideFacility> GetOutsideFacility(int id)
    {
        return await Single<OutsideFacility>(u => u.Id == id);
    }

    public async Task<List<OutsideFacility>> GetOutsideFacilities(Expression<Func<OutsideFacility, bool>> filter = null,
        Func<IQueryable<OutsideFacility>, IOrderedQueryable<OutsideFacility>> orderBy = null,
        Func<IQueryable<OutsideFacility>, IIncludableQueryable<OutsideFacility, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}