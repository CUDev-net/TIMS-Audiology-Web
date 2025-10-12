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

public interface IHaComponentUnitOfWork : IUnitOfWork
{
    Task<HaComponent> GetHaComponent(int id);

    Task<List<HaComponent>> GetHaComponents(Expression<Func<HaComponent, bool>> filter = null,
        Func<IQueryable<HaComponent>, IOrderedQueryable<HaComponent>> orderBy = null,
        Func<IQueryable<HaComponent>, IIncludableQueryable<HaComponent, object>> includes = null);
}

public class HaComponentUnitOfWork : UnitOfWorkBase, IHaComponentUnitOfWork
{
    public HaComponentUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "HAAttachmentType";

    public async Task<HaComponent> GetHaComponent(int id)
    {
        return await Single<HaComponent>(u => u.Id == id);
    }

    public async Task<List<HaComponent>> GetHaComponents(Expression<Func<HaComponent, bool>> filter = null,
        Func<IQueryable<HaComponent>, IOrderedQueryable<HaComponent>> orderBy = null,
        Func<IQueryable<HaComponent>, IIncludableQueryable<HaComponent, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}