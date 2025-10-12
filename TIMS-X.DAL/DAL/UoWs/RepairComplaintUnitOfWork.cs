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

public interface IRepairComplaintUnitOfWork : IUnitOfWork
{
    Task<RepairComplaint> GetRepairComplaint(int id);

    Task<List<RepairComplaint>> GetRepairComplaints(Expression<Func<RepairComplaint, bool>> filter = null,
        Func<IQueryable<RepairComplaint>, IOrderedQueryable<RepairComplaint>> orderBy = null,
        Func<IQueryable<RepairComplaint>, IIncludableQueryable<RepairComplaint, object>> includes = null);
}

public class RepairComplaintUnitOfWork : UnitOfWorkBase, IRepairComplaintUnitOfWork
{
    public RepairComplaintUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(RepairComplaint);

    public async Task<RepairComplaint> GetRepairComplaint(int id)
    {
        return await Single<RepairComplaint>(u => u.Id == id);
    }

    public async Task<List<RepairComplaint>> GetRepairComplaints(Expression<Func<RepairComplaint, bool>> filter = null,
        Func<IQueryable<RepairComplaint>, IOrderedQueryable<RepairComplaint>> orderBy = null,
        Func<IQueryable<RepairComplaint>, IIncludableQueryable<RepairComplaint, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}