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

public interface IMedicationUnitOfWork : IUnitOfWork
{
    Task<Medication> GetMedication(int id);

    Task<List<Medication>> GetMedications(Expression<Func<Medication, bool>> filter = null,
        Func<IQueryable<Medication>, IOrderedQueryable<Medication>> orderBy = null,
        Func<IQueryable<Medication>, IIncludableQueryable<Medication, object>> includes = null);
}

public class MedicationUnitOfWork : UnitOfWorkBase, IMedicationUnitOfWork
{
    public MedicationUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(Medication);

    public async Task<Medication> GetMedication(int id)
    {
        return await Single<Medication>(u => u.Id == id);
    }

    public async Task<List<Medication>> GetMedications(Expression<Func<Medication, bool>> filter = null,
        Func<IQueryable<Medication>, IOrderedQueryable<Medication>> orderBy = null,
        Func<IQueryable<Medication>, IIncludableQueryable<Medication, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}