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

public interface ISubmitterInfoUnitOfWork : IUnitOfWork
{
    Task<SubmitterInfo> GetSubmitterInfo(int id);

    Task<List<SubmitterInfo>> GetSubmitterInfos(Expression<Func<SubmitterInfo, bool>> filter = null,
        Func<IQueryable<SubmitterInfo>, IOrderedQueryable<SubmitterInfo>> orderBy = null,
        Func<IQueryable<SubmitterInfo>, IIncludableQueryable<SubmitterInfo, object>> includes = null);
}

public class SubmitterInfoUnitOfWork : UnitOfWorkBase, ISubmitterInfoUnitOfWork
{
    public SubmitterInfoUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(SubmitterInfo);

    public async Task<SubmitterInfo> GetSubmitterInfo(int id)
    {
        return await Single<SubmitterInfo>(u => u.Id == id);
    }

    public async Task<List<SubmitterInfo>> GetSubmitterInfos(Expression<Func<SubmitterInfo, bool>> filter = null,
        Func<IQueryable<SubmitterInfo>, IOrderedQueryable<SubmitterInfo>> orderBy = null,
        Func<IQueryable<SubmitterInfo>, IIncludableQueryable<SubmitterInfo, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}