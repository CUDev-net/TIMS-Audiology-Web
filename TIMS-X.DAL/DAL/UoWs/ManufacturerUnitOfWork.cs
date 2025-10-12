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

public interface IManufacturerUnitOfWork : IUnitOfWork
{
    Task<Manufacturer> GetManufacturer(int id);

    Task<List<Manufacturer>> GetManufacturers(Expression<Func<Manufacturer, bool>> filter = null,
        Func<IQueryable<Manufacturer>, IOrderedQueryable<Manufacturer>> orderBy = null,
        Func<IQueryable<Manufacturer>, IIncludableQueryable<Manufacturer, object>> includes = null);
}

public class ManufacturerUnitOfWork : UnitOfWorkBase, IManufacturerUnitOfWork
{
    public ManufacturerUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(Manufacturer);

    public async Task<Manufacturer> GetManufacturer(int id)
    {
        return await Single<Manufacturer>(u => u.Id == id);
    }

    public async Task<List<Manufacturer>> GetManufacturers(Expression<Func<Manufacturer, bool>> filter = null,
        Func<IQueryable<Manufacturer>, IOrderedQueryable<Manufacturer>> orderBy = null,
        Func<IQueryable<Manufacturer>, IIncludableQueryable<Manufacturer, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}