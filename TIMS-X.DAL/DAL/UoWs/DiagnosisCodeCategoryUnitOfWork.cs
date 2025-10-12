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

public interface IDiagnosisCodeCategoryUnitOfWork : IUnitOfWork
{
    Task<DiagnosisCodeCategory> GetDiagnosisCodeCategory(int id);

    Task<List<DiagnosisCodeCategory>> GetDiagnosisCodeCategories(
        Expression<Func<DiagnosisCodeCategory, bool>> filter = null,
        Func<IQueryable<DiagnosisCodeCategory>, IOrderedQueryable<DiagnosisCodeCategory>> orderBy = null,
        Func<IQueryable<DiagnosisCodeCategory>, IIncludableQueryable<DiagnosisCodeCategory, object>> includes = null);
}

public class DiagnosisCodeCategoryUnitOfWork : UnitOfWorkBase, IDiagnosisCodeCategoryUnitOfWork
{
    public DiagnosisCodeCategoryUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(
        context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(DiagnosisCodeCategory);

    public async Task<DiagnosisCodeCategory> GetDiagnosisCodeCategory(int id)
    {
        return await Single<DiagnosisCodeCategory>(u => u.Id == id);
    }

    public async Task<List<DiagnosisCodeCategory>> GetDiagnosisCodeCategories(
        Expression<Func<DiagnosisCodeCategory, bool>> filter = null,
        Func<IQueryable<DiagnosisCodeCategory>, IOrderedQueryable<DiagnosisCodeCategory>> orderBy = null,
        Func<IQueryable<DiagnosisCodeCategory>, IIncludableQueryable<DiagnosisCodeCategory, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}