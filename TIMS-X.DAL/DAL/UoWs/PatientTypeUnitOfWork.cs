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

public interface IPatientTypeUnitOfWork : IUnitOfWork
{
    Task<PatientType> GetPatientType(int id);

    Task<List<PatientType>> GetPatientTypes(Expression<Func<PatientType, bool>> filter = null,
        Func<IQueryable<PatientType>, IOrderedQueryable<PatientType>> orderBy = null,
        Func<IQueryable<PatientType>, IIncludableQueryable<PatientType, object>> includes = null);
}

public class PatientTypeUnitOfWork : UnitOfWorkBase, IPatientTypeUnitOfWork
{
    public PatientTypeUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(PatientType);

    public async Task<PatientType> GetPatientType(int id)
    {
        return await Single<PatientType>(u => u.Id == id);
    }

    public async Task<List<PatientType>> GetPatientTypes(Expression<Func<PatientType, bool>> filter = null,
        Func<IQueryable<PatientType>, IOrderedQueryable<PatientType>> orderBy = null,
        Func<IQueryable<PatientType>, IIncludableQueryable<PatientType, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}