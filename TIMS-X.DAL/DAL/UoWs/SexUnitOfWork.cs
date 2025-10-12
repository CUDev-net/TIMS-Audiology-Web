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

public interface ISexUnitOfWork : IUnitOfWork
{
    Task<Sex> GetSex(int id);

    Task<List<Sex>> GetSexes(Expression<Func<Sex, bool>> filter = null,
        Func<IQueryable<Sex>, IOrderedQueryable<Sex>> orderBy = null,
        Func<IQueryable<Sex>, IIncludableQueryable<Sex, object>> includes = null);
}

public class SexUnitOfWork : UnitOfWorkBase, ISexUnitOfWork
{
    public SexUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(Sex);

    public async Task<Sex> GetSex(int id)
    {
        return await Single<Sex>(u => u.Id == id);
    }

    public async Task<List<Sex>> GetSexes(Expression<Func<Sex, bool>> filter = null,
        Func<IQueryable<Sex>, IOrderedQueryable<Sex>> orderBy = null,
        Func<IQueryable<Sex>, IIncludableQueryable<Sex, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}