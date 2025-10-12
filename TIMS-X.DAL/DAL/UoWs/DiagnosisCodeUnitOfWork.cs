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

public interface IDiagnosisCodeUnitOfWork : IUnitOfWork
{
    Task<DiagnosisCode> GetDiagnosisCode(int id);

    Task<List<DiagnosisCode>> GetDiagnosisCodes(Expression<Func<DiagnosisCode, bool>> filter = null,
        Func<IQueryable<DiagnosisCode>, IOrderedQueryable<DiagnosisCode>> orderBy = null,
        Func<IQueryable<DiagnosisCode>, IIncludableQueryable<DiagnosisCode, object>> includes = null);
}

public class DiagnosisCodeUnitOfWork : UnitOfWorkBase, IDiagnosisCodeUnitOfWork
{
    public DiagnosisCodeUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(DiagnosisCode);

    public async Task<DiagnosisCode> GetDiagnosisCode(int id)
    {
        return await Single<DiagnosisCode>(u => u.Id == id);
    }

    public async Task<List<DiagnosisCode>> GetDiagnosisCodes(Expression<Func<DiagnosisCode, bool>> filter = null,
        Func<IQueryable<DiagnosisCode>, IOrderedQueryable<DiagnosisCode>> orderBy = null,
        Func<IQueryable<DiagnosisCode>, IIncludableQueryable<DiagnosisCode, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}