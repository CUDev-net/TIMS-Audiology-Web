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

public interface IBatterySizeUnitOfWork : IUnitOfWork
{
    Task<BatterySize> GetBatterySize(int id);

    Task<List<BatterySize>> GetBatterySizes(Expression<Func<BatterySize, bool>> filter = null,
        Func<IQueryable<BatterySize>, IOrderedQueryable<BatterySize>> orderBy = null,
        Func<IQueryable<BatterySize>, IIncludableQueryable<BatterySize, object>> includes = null);
}

public class BatterySizeUnitOfWork : UnitOfWorkBase, IBatterySizeUnitOfWork
{
    public BatterySizeUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(BatterySize);

    public async Task<BatterySize> GetBatterySize(int id)
    {
        return await Single<BatterySize>(u => u.Id == id);
    }

    public async Task<List<BatterySize>> GetBatterySizes(Expression<Func<BatterySize, bool>> filter = null,
        Func<IQueryable<BatterySize>, IOrderedQueryable<BatterySize>> orderBy = null,
        Func<IQueryable<BatterySize>, IIncludableQueryable<BatterySize, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}