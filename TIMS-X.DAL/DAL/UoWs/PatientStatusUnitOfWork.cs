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

public interface IPatientStatusUnitOfWork : IUnitOfWork
{
    Task<PatientStatus> GetPatientStatus(int id);

    Task<List<PatientStatus>> GetPatientStatuses(Expression<Func<PatientStatus, bool>> filter = null,
        Func<IQueryable<PatientStatus>, IOrderedQueryable<PatientStatus>> orderBy = null,
        Func<IQueryable<PatientStatus>, IIncludableQueryable<PatientStatus, object>> includes = null);
}

public class PatientStatusUnitOfWork : UnitOfWorkBase, IPatientStatusUnitOfWork
{
    public PatientStatusUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(PatientStatus);

    public async Task<PatientStatus> GetPatientStatus(int id)
    {
        return await Single<PatientStatus>(u => u.Id == id);
    }

    public async Task<List<PatientStatus>> GetPatientStatuses(Expression<Func<PatientStatus, bool>> filter = null,
        Func<IQueryable<PatientStatus>, IOrderedQueryable<PatientStatus>> orderBy = null,
        Func<IQueryable<PatientStatus>, IIncludableQueryable<PatientStatus, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}