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

public interface IHaWarrantyTypeUnitOfWork : IUnitOfWork
{
    Task<HaWarrantyType> GetHaWarrantyType(int id);

    Task<List<HaWarrantyType>> GetHaWarrantyTypes(
        Expression<Func<HaWarrantyType, bool>> filter = null,
        Func<IQueryable<HaWarrantyType>, IOrderedQueryable<HaWarrantyType>> orderBy = null,
        Func<IQueryable<HaWarrantyType>, IIncludableQueryable<HaWarrantyType, object>> includes = null);
}

public class HaWarrantyTypeUnitOfWork : UnitOfWorkBase, IHaWarrantyTypeUnitOfWork
{
    public HaWarrantyTypeUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "WarrantyType";

    public async Task<HaWarrantyType> GetHaWarrantyType(int id)
    {
        return await Single<HaWarrantyType>(u => u.Id == id);
    }

    public async Task<List<HaWarrantyType>> GetHaWarrantyTypes(
        Expression<Func<HaWarrantyType, bool>> filter = null,
        Func<IQueryable<HaWarrantyType>, IOrderedQueryable<HaWarrantyType>> orderBy = null,
        Func<IQueryable<HaWarrantyType>, IIncludableQueryable<HaWarrantyType, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}