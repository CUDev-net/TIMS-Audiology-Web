using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IPatientRequiredFieldUnitOfWork : IUnitOfWork
{
    IQueryable<PatientRequiredField> GetPatientRequiredFields(Expression<Func<PatientRequiredField, bool>> filter = null,
        Func<IQueryable<PatientRequiredField>, IOrderedQueryable<PatientRequiredField>> orderBy = null,
        Func<IQueryable<PatientRequiredField>, IIncludableQueryable<PatientRequiredField, object>> includes = null);
}

public class PatientRequiredFieldUnitOfWork : UnitOfWorkBase, IPatientRequiredFieldUnitOfWork
{
    public PatientRequiredFieldUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(PatientRequiredField);

    public IQueryable<PatientRequiredField> GetPatientRequiredFields(Expression<Func<PatientRequiredField, bool>> filter = null,
        Func<IQueryable<PatientRequiredField>, IOrderedQueryable<PatientRequiredField>> orderBy = null,
        Func<IQueryable<PatientRequiredField>, IIncludableQueryable<PatientRequiredField, object>> includes = null)
    {
        return Get(filter, orderBy, includes);
    }
}