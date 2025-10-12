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

public interface IHaReturnReasonUnitOfWork : IUnitOfWork
{
    Task<HaReturnReason> GetReturnReason(int id);

    Task<List<HaReturnReason>> GetReturnReasons(Expression<Func<HaReturnReason, bool>> filter = null,
        Func<IQueryable<HaReturnReason>, IOrderedQueryable<HaReturnReason>> orderBy = null,
        Func<IQueryable<HaReturnReason>, IIncludableQueryable<HaReturnReason, object>> includes = null);
}

public class HaReturnReasonUnitOfWork : UnitOfWorkBase, IHaReturnReasonUnitOfWork
{
    public HaReturnReasonUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "HAReturnReason";

    public async Task<HaReturnReason> GetReturnReason(int id)
    {
        return await Single<HaReturnReason>(u => u.Id == id);
    }

    public async Task<List<HaReturnReason>> GetReturnReasons(Expression<Func<HaReturnReason, bool>> filter = null,
        Func<IQueryable<HaReturnReason>, IOrderedQueryable<HaReturnReason>> orderBy = null,
        Func<IQueryable<HaReturnReason>, IIncludableQueryable<HaReturnReason, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}