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

public interface IMedicalConditionUnitOfWork : IUnitOfWork
{
    Task<MedicalCondition> GetMedicalCondition(int id);

    Task<List<MedicalCondition>> GetMedicalConditions(Expression<Func<MedicalCondition, bool>> filter = null,
        Func<IQueryable<MedicalCondition>, IOrderedQueryable<MedicalCondition>> orderBy = null,
        Func<IQueryable<MedicalCondition>, IIncludableQueryable<MedicalCondition, object>> includes = null);
}

public class MedicalConditionUnitOfWork : UnitOfWorkBase, IMedicalConditionUnitOfWork
{
    public MedicalConditionUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(MedicalCondition);

    public async Task<MedicalCondition> GetMedicalCondition(int id)
    {
        return await Single<MedicalCondition>(u => u.Id == id);
    }

    public async Task<List<MedicalCondition>> GetMedicalConditions(
        Expression<Func<MedicalCondition, bool>> filter = null,
        Func<IQueryable<MedicalCondition>, IOrderedQueryable<MedicalCondition>> orderBy = null,
        Func<IQueryable<MedicalCondition>, IIncludableQueryable<MedicalCondition, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}