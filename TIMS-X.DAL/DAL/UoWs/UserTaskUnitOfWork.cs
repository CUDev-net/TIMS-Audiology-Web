using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IUserTaskUnitOfWork : IUnitOfWork
{
    Task<UserTask> GetUserTask(int id);

    IQueryable<UserTask> GetUserTasks(Expression<Func<UserTask, bool>> filter = null,
        Func<IQueryable<UserTask>, IOrderedQueryable<UserTask>> orderBy = null,
        Func<IQueryable<UserTask>, IIncludableQueryable<UserTask, object>> includes = null);
}

public class UserTaskUnitOfWork : UnitOfWorkBase, IUserTaskUnitOfWork
{
    public UserTaskUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "TIMSUserTask";

    public async Task<UserTask> GetUserTask(int id)
    {
        return await Single<UserTask>(u => u.Id == id);
    }

    public IQueryable<UserTask> GetUserTasks(Expression<Func<UserTask, bool>> filter = null,
        Func<IQueryable<UserTask>, IOrderedQueryable<UserTask>> orderBy = null,
        Func<IQueryable<UserTask>, IIncludableQueryable<UserTask, object>> includes = null)
    {
        return Get(filter, orderBy, includes);
    }
}