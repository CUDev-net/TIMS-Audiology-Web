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

public interface IResourceUnitOfWork : IUnitOfWork
{
    Task<Resource> GetResource(int id);

    Task<List<Resource>> GetResources(Expression<Func<Resource, bool>> filter = null,
        Func<IQueryable<Resource>, IOrderedQueryable<Resource>> orderBy = null,
        Func<IQueryable<Resource>, IIncludableQueryable<Resource, object>> includes = null);
}

public class ResourceUnitOfWork : UnitOfWorkBase, IResourceUnitOfWork
{
    public ResourceUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(Resource);

    public async Task<Resource> GetResource(int id)
    {
        return await Single<Resource>(u => u.Id == id);
    }

    public async Task<List<Resource>> GetResources(Expression<Func<Resource, bool>> filter = null,
        Func<IQueryable<Resource>, IOrderedQueryable<Resource>> orderBy = null,
        Func<IQueryable<Resource>, IIncludableQueryable<Resource, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}