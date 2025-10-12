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

public interface IHaModelUnitOfWork : IUnitOfWork
{
    Task<HaModel> GetHaModel(int id, Func<IQueryable<HaModel>, IIncludableQueryable<HaModel, object>> includes = null);

    Task<List<HaModel>> GetHaModels(Expression<Func<HaModel, bool>> filter = null,
        Func<IQueryable<HaModel>, IOrderedQueryable<HaModel>> orderBy = null,
        Func<IQueryable<HaModel>, IIncludableQueryable<HaModel, object>> includes = null);
}

public class HaModelUnitOfWork : UnitOfWorkBase, IHaModelUnitOfWork
{
    public HaModelUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "HAModel";

    public async Task<HaModel> GetHaModel(int id,
        Func<IQueryable<HaModel>, IIncludableQueryable<HaModel, object>> includes = null)
    {
        return await Single(h => h.Id == id, includes);
    }

    public async Task<List<HaModel>> GetHaModels(Expression<Func<HaModel, bool>> filter = null,
        Func<IQueryable<HaModel>, IOrderedQueryable<HaModel>> orderBy = null,
        Func<IQueryable<HaModel>, IIncludableQueryable<HaModel, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}