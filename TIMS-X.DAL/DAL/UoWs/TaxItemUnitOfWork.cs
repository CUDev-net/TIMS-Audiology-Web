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

public interface ITaxItemUnitOfWork : IUnitOfWork
{
    Task<TaxItem> GetTaxItem(int id, Func<IQueryable<TaxItem>, IIncludableQueryable<TaxItem, object>> includes = null);

    Task<List<TaxItem>> GetTaxItems(Expression<Func<TaxItem, bool>> filter = null,
        Func<IQueryable<TaxItem>, IOrderedQueryable<TaxItem>> orderBy = null,
        Func<IQueryable<TaxItem>, IIncludableQueryable<TaxItem, object>> includes = null);
}

public class TaxItemUnitOfWork : UnitOfWorkBase, ITaxItemUnitOfWork
{
    public TaxItemUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)

    {
    }

    protected override string TableName => "POSTaxItem";

    public async Task<TaxItem> GetTaxItem(int id, Func<IQueryable<TaxItem>, IIncludableQueryable<TaxItem, object>> includes = null)
    {
        return await Single<TaxItem>(u => u.Id == id, includes);
    }

    public async Task<List<TaxItem>> GetTaxItems(Expression<Func<TaxItem, bool>> filter = null,
        Func<IQueryable<TaxItem>, IOrderedQueryable<TaxItem>> orderBy = null,
        Func<IQueryable<TaxItem>, IIncludableQueryable<TaxItem, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}