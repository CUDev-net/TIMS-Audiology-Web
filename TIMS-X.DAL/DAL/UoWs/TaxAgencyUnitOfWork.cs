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

public interface ITaxAgencyUnitOfWork : IUnitOfWork
{
    Task<TaxAgency> GetTaxAgency(int id);

    Task<List<TaxAgency>> GetTaxAgencies(Expression<Func<TaxAgency, bool>> filter = null,
        Func<IQueryable<TaxAgency>, IOrderedQueryable<TaxAgency>> orderBy = null,
        Func<IQueryable<TaxAgency>, IIncludableQueryable<TaxAgency, object>> includes = null);
}

public class TaxAgencyUnitOfWork : UnitOfWorkBase, ITaxAgencyUnitOfWork
{
    public TaxAgencyUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)

    {
    }

    protected override string TableName => "POSTaxAgency";

    public async Task<TaxAgency> GetTaxAgency(int id)
    {
        return await Single<TaxAgency>(u => u.Id == id);
    }

    public async Task<List<TaxAgency>> GetTaxAgencies(Expression<Func<TaxAgency, bool>> filter = null,
        Func<IQueryable<TaxAgency>, IOrderedQueryable<TaxAgency>> orderBy = null,
        Func<IQueryable<TaxAgency>, IIncludableQueryable<TaxAgency, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}