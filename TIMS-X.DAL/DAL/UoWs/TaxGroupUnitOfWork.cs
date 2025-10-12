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

public interface ITaxGroupUnitOfWork : IUnitOfWork
{
    Task<TaxGroup> GetTaxGroup(int id);

    Task<List<TaxGroup>> GetTaxGroups(Expression<Func<TaxGroup, bool>> filter = null,
        Func<IQueryable<TaxGroup>, IOrderedQueryable<TaxGroup>> orderBy = null,
        Func<IQueryable<TaxGroup>, IIncludableQueryable<TaxGroup, object>> includes = null);
}

public class TaxGroupUnitOfWork : UnitOfWorkBase, ITaxGroupUnitOfWork
{
    public TaxGroupUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "POSTaxGroup";

    public async Task<TaxGroup> GetTaxGroup(int id)
    {
        return await Single<TaxGroup>(u => u.Id == id);
    }

    public async Task<List<TaxGroup>> GetTaxGroups(Expression<Func<TaxGroup, bool>> filter = null,
        Func<IQueryable<TaxGroup>, IOrderedQueryable<TaxGroup>> orderBy = null,
        Func<IQueryable<TaxGroup>, IIncludableQueryable<TaxGroup, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}