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

public interface IPatientInsuranceUnitOfWork : IUnitOfWork
{
    Task<PatientInsurance> GetPatientInsurance(int id);

    Task<List<PatientInsurance>> GetPatientInsurancees(
        Expression<Func<PatientInsurance, bool>> filter = null,
        Func<IQueryable<PatientInsurance>, IOrderedQueryable<PatientInsurance>> orderBy = null,
        Func<IQueryable<PatientInsurance>, IIncludableQueryable<PatientInsurance, object>> includes = null);
}

public class PatientInsuranceUnitOfWork : UnitOfWorkBase, IPatientInsuranceUnitOfWork
{
    public PatientInsuranceUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "PatientInsurance";

    public async Task<PatientInsurance> GetPatientInsurance(int id)
    {
        return await Single<PatientInsurance>(u => u.Id == id);
    }

    public async Task<List<PatientInsurance>> GetPatientInsurancees(
        Expression<Func<PatientInsurance, bool>> filter = null,
        Func<IQueryable<PatientInsurance>, IOrderedQueryable<PatientInsurance>> orderBy = null,
        Func<IQueryable<PatientInsurance>, IIncludableQueryable<PatientInsurance, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}