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

public interface ISalutationUnitOfWork : IUnitOfWork
{
    Task<Salutation> GetSalutation(int id);

    Task<List<Salutation>> GetSalutations(Expression<Func<Salutation, bool>> filter = null,
        Func<IQueryable<Salutation>, IOrderedQueryable<Salutation>> orderBy = null,
        Func<IQueryable<Salutation>, IIncludableQueryable<Salutation, object>> includes = null);
}

public class SalutationUnitOfWork : UnitOfWorkBase, ISalutationUnitOfWork
{
    public SalutationUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(Salutation);

    public async Task<Salutation> GetSalutation(int id)
    {
        return await Single<Salutation>(u => u.Id == id);
    }

    public async Task<List<Salutation>> GetSalutations(Expression<Func<Salutation, bool>> filter = null,
        Func<IQueryable<Salutation>, IOrderedQueryable<Salutation>> orderBy = null,
        Func<IQueryable<Salutation>, IIncludableQueryable<Salutation, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}