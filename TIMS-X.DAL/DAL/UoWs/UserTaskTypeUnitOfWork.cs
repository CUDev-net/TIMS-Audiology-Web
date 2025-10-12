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

public interface IUserTaskTypeUnitOfWork : IUnitOfWork
{
    Task<UserTaskType> GetUserTaskType(int id);

    Task<List<UserTaskType>> GetUserTaskTypes(Expression<Func<UserTaskType, bool>> filter = null,
        Func<IQueryable<UserTaskType>, IOrderedQueryable<UserTaskType>> orderBy = null,
        Func<IQueryable<UserTaskType>, IIncludableQueryable<UserTaskType, object>> includes = null);
}

public class UserTaskTypeUnitOfWork : UnitOfWorkBase, IUserTaskTypeUnitOfWork
{
    public UserTaskTypeUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "TIMSUserTaskType";

    public async Task<UserTaskType> GetUserTaskType(int id)
    {
        return await Single<UserTaskType>(u => u.Id == id);
    }

    public async Task<List<UserTaskType>> GetUserTaskTypes(Expression<Func<UserTaskType, bool>> filter = null,
        Func<IQueryable<UserTaskType>, IOrderedQueryable<UserTaskType>> orderBy = null,
        Func<IQueryable<UserTaskType>, IIncludableQueryable<UserTaskType, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}