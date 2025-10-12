using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IHaModelOptionUnitOfWork : IUnitOfWork
{
    Task<HaModelOption> GetHaModelOption(int id);

    IQueryable<HaModelOption> GetHaModelOptions(Expression<Func<HaModelOption, bool>> filter = null,
        Func<IQueryable<HaModelOption>, IOrderedQueryable<HaModelOption>> orderBy = null,
        Func<IQueryable<HaModelOption>, IIncludableQueryable<HaModelOption, object>> includes = null);
}

public class HaModelOptionUnitOfWork : UnitOfWorkBase, IHaModelOptionUnitOfWork
{
    public HaModelOptionUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "HAModelOption";

    public async Task<HaModelOption> GetHaModelOption(int id)
    {
        return await Single<HaModelOption>(u => u.Id == id);
    }

    public IQueryable<HaModelOption> GetHaModelOptions(Expression<Func<HaModelOption, bool>> filter = null,
        Func<IQueryable<HaModelOption>, IOrderedQueryable<HaModelOption>> orderBy = null,
        Func<IQueryable<HaModelOption>, IIncludableQueryable<HaModelOption, object>> includes = null)
    {
        return Get(filter, orderBy, includes);
    }
}