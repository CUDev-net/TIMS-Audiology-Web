using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IHistoryUnitOfWork : IUnitOfWork
{

    void DeleteByAppointmentId(int id);

    Task<History> GetHistory(int id,
        Func<IQueryable<History>, IIncludableQueryable<History, object>> includes = null);

    Task<List<History>> GetHistories(Expression<Func<History, bool>> filter = null,
        Func<IQueryable<History>, IOrderedQueryable<History>> orderBy = null,
        Func<IQueryable<History>, IIncludableQueryable<History, object>> includes = null);
}

public class HistoryUnitOfWork : UnitOfWorkBase, IHistoryUnitOfWork
{
    public HistoryUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(History);

    public async Task<History> GetHistory(int id,
        Func<IQueryable<History>, IIncludableQueryable<History, object>> includes = null)
    {
        return await Single(u => u.Id == id, includes);
    }

    public async Task<List<History>> GetHistories(Expression<Func<History, bool>> filter = null,
        Func<IQueryable<History>, IOrderedQueryable<History>> orderBy = null,
        Func<IQueryable<History>, IIncludableQueryable<History, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }

    public void DeleteByAppointmentId(int id)
    {
        object[] sqlParams =
        {
            new SqlParameter("@id", id)
        };
        ExecuteSqlCommand("delete from history where AppointmentID = @id", sqlParams);
    }
}