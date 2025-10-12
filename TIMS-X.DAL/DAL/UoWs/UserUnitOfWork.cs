using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IUserUnitOfWork : IUnitOfWork
{
    Task<User> GetUser(long id);

    IQueryable<User> GetUsers(Expression<Func<User, bool>> filter = null,
        Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null,
        Func<IQueryable<User>, IIncludableQueryable<User, object>> includes = null);
}

public class UserUnitOfWork : UnitOfWorkBase, IUserUnitOfWork
{
    public UserUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "TIMSUser";

    public async Task<User> GetUser(long id)
    {
        return await Single<User>(u => u.Id == id);
    }

    public IQueryable<User> GetUsers(Expression<Func<User, bool>> filter = null,
        Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null,
        Func<IQueryable<User>, IIncludableQueryable<User, object>> includes = null)
    {
        return Get(filter, orderBy, includes);
    }
}