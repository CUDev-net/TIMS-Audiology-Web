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

public interface IInsurancePayerUnitOfWork : IUnitOfWork
{
    Task<InsurancePayer> GetInsurancePayer(int id);

    Task<List<InsurancePayer>> GetInsurancePayers(Expression<Func<InsurancePayer, bool>> filter = null,
        Func<IQueryable<InsurancePayer>, IOrderedQueryable<InsurancePayer>> orderBy = null,
        Func<IQueryable<InsurancePayer>, IIncludableQueryable<InsurancePayer, object>> includes = null);
}

public class InsurancePayerUnitOfWork : UnitOfWorkBase, IInsurancePayerUnitOfWork
{
    public InsurancePayerUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "InsuranceCarrier";

    public async Task<InsurancePayer> GetInsurancePayer(int id)
    {
        return await Single<InsurancePayer>(u => u.Id == id);
    }

    public async Task<List<InsurancePayer>> GetInsurancePayers(Expression<Func<InsurancePayer, bool>> filter = null,
        Func<IQueryable<InsurancePayer>, IOrderedQueryable<InsurancePayer>> orderBy = null,
        Func<IQueryable<InsurancePayer>, IIncludableQueryable<InsurancePayer, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}