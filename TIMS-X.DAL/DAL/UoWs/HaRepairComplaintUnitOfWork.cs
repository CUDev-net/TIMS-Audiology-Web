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

public interface IHaRepairComplaintUnitOfWork : IUnitOfWork
{
    Task<HaRepairComplaint> GetHaRepairComplaint(int id);

    Task<List<HaRepairComplaint>> GetHaRepairComplaints(Expression<Func<HaRepairComplaint, bool>> filter = null,
        Func<IQueryable<HaRepairComplaint>, IOrderedQueryable<HaRepairComplaint>> orderBy = null,
        Func<IQueryable<HaRepairComplaint>, IIncludableQueryable<HaRepairComplaint, object>> includes = null);
}

public class HaRepairComplaintUnitOfWork : UnitOfWorkBase, IHaRepairComplaintUnitOfWork
{
    public HaRepairComplaintUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "RepairComplaint";

    public async Task<HaRepairComplaint> GetHaRepairComplaint(int id)
    {
        return await Single<HaRepairComplaint>(u => u.Id == id);
    }

    public async Task<List<HaRepairComplaint>> GetHaRepairComplaints(
        Expression<Func<HaRepairComplaint, bool>> filter = null,
        Func<IQueryable<HaRepairComplaint>, IOrderedQueryable<HaRepairComplaint>> orderBy = null,
        Func<IQueryable<HaRepairComplaint>, IIncludableQueryable<HaRepairComplaint, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}