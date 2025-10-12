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

public interface IAuthorizationUnitOfWork : IUnitOfWork
{
    Task<Authorization> GetAuthorization(int id);

    Task<List<Authorization>> GetAuthorizations(Expression<Func<Authorization, bool>> filter = null,
        Func<IQueryable<Authorization>, IOrderedQueryable<Authorization>> orderBy = null,
        Func<IQueryable<Authorization>, IIncludableQueryable<Authorization, object>> includes = null);
}

public class AuthorizationUnitOfWork : UnitOfWorkBase, IAuthorizationUnitOfWork
{
    public AuthorizationUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(Authorization);

    public async Task<Authorization> GetAuthorization(int id)
    {
        return await Single<Authorization>(u => u.Id == id);
    }

    public async Task<List<Authorization>> GetAuthorizations(Expression<Func<Authorization, bool>> filter = null,
        Func<IQueryable<Authorization>, IOrderedQueryable<Authorization>> orderBy = null,
        Func<IQueryable<Authorization>, IIncludableQueryable<Authorization, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}