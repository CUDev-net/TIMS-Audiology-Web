using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IHaReturnUnitOfWork : IUnitOfWork
{
    Task<HaReturn> GetReturnByHaHistory(int haHistoryId);

    IQueryable<HaReturn> GetReturns(Expression<Func<HaReturn, bool>> filter = null,
        Func<IQueryable<HaReturn>, IOrderedQueryable<HaReturn>> orderBy = null,
        Func<IQueryable<HaReturn>, IIncludableQueryable<HaReturn, object>> includes = null);
}

public class HaReturnUnitOfWork : UnitOfWorkBase, IHaReturnUnitOfWork
{
    public HaReturnUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    private IIncludableQueryable<HaReturn, object> _Includes(IQueryable<HaReturn> h)
    {
        return h.Include(x => x.ReturnReason);
    }

    protected override string TableName => "HAReturn";

    public async Task<HaReturn> GetReturnByHaHistory(int haHistoryId)
    {
        return await Single<HaReturn>(u => u.HaHistoryId == haHistoryId, _Includes);
    }

    public IQueryable<HaReturn> GetReturns(Expression<Func<HaReturn, bool>> filter = null,
        Func<IQueryable<HaReturn>, IOrderedQueryable<HaReturn>> orderBy = null,
        Func<IQueryable<HaReturn>, IIncludableQueryable<HaReturn, object>> includes = null)
    {
        return Get(filter, orderBy, includes);
    }
}