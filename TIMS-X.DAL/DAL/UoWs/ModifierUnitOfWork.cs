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

public interface IModifierUnitOfWork : IUnitOfWork
{
    Task<Modifier> GetModifier(int id);

    Task<List<Modifier>> GetModifiers(Expression<Func<Modifier, bool>> filter = null,
        Func<IQueryable<Modifier>, IOrderedQueryable<Modifier>> orderBy = null,
        Func<IQueryable<Modifier>, IIncludableQueryable<Modifier, object>> includes = null);
}

public class ModifierUnitOfWork : UnitOfWorkBase, IModifierUnitOfWork
{
    public ModifierUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(Modifier);

    public async Task<Modifier> GetModifier(int id)
    {
        return await Single<Modifier>(u => u.Id == id);
    }

    public async Task<List<Modifier>> GetModifiers(Expression<Func<Modifier, bool>> filter = null,
        Func<IQueryable<Modifier>, IOrderedQueryable<Modifier>> orderBy = null,
        Func<IQueryable<Modifier>, IIncludableQueryable<Modifier, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}