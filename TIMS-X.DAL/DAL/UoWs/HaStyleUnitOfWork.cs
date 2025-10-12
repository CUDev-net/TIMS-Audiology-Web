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

public interface IHaStyleUnitOfWork : IUnitOfWork
{
    Task<HaStyle> GetHaStyle(int id);

    Task<List<HaStyle>> GetHaStyles(
        Expression<Func<HaStyle, bool>> filter = null,
        Func<IQueryable<HaStyle>, IOrderedQueryable<HaStyle>> orderBy = null,
        Func<IQueryable<HaStyle>, IIncludableQueryable<HaStyle, object>> includes = null);
}

public class HaStyleUnitOfWork : UnitOfWorkBase, IHaStyleUnitOfWork
{
    public HaStyleUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "HAStyle";

    public async Task<HaStyle> GetHaStyle(int id)
    {
        return await Single<HaStyle>(u => u.Id == id);
    }

    public async Task<List<HaStyle>> GetHaStyles(
        Expression<Func<HaStyle, bool>> filter = null,
        Func<IQueryable<HaStyle>, IOrderedQueryable<HaStyle>> orderBy = null,
        Func<IQueryable<HaStyle>, IIncludableQueryable<HaStyle, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}