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

public interface IAdjustmentTypeUnitOfWork : IUnitOfWork
{
    Task<AdjustmentType> GetAdjustmentType(int id);

    Task<List<AdjustmentType>> GetAdjustmentTypes(
        Expression<Func<AdjustmentType, bool>> filter = null,
        Func<IQueryable<AdjustmentType>, IOrderedQueryable<AdjustmentType>> orderBy = null,
        Func<IQueryable<AdjustmentType>, IIncludableQueryable<AdjustmentType, object>> includes = null);
}

public class AdjustmentTypeUnitOfWork : UnitOfWorkBase, IAdjustmentTypeUnitOfWork
{
    public AdjustmentTypeUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(AdjustmentType);

    public async Task<AdjustmentType> GetAdjustmentType(int id)
    {
        return await Single<AdjustmentType>(u => u.Id == id);
    }

    public async Task<List<AdjustmentType>> GetAdjustmentTypes(
        Expression<Func<AdjustmentType, bool>> filter = null,
        Func<IQueryable<AdjustmentType>, IOrderedQueryable<AdjustmentType>> orderBy = null,
        Func<IQueryable<AdjustmentType>, IIncludableQueryable<AdjustmentType, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}