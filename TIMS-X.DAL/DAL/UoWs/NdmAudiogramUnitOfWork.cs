using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface INdmAudiogramUnitOfWork : IUnitOfWork
{
    IQueryable<NdmAudiogram> GetAudiograms(Expression<Func<NdmAudiogram, bool>> filter = null,
        Func<IQueryable<NdmAudiogram>, IOrderedQueryable<NdmAudiogram>> orderBy = null,
        Func<IQueryable<NdmAudiogram>, IIncludableQueryable<NdmAudiogram, object>> includes = null);
}

public class NdmAudiogramUnitOfWork : UnitOfWorkBase, INdmAudiogramUnitOfWork
{
    public NdmAudiogramUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "NDMAudiogram";

    public IQueryable<NdmAudiogram> GetAudiograms(Expression<Func<NdmAudiogram, bool>> filter = null,
        Func<IQueryable<NdmAudiogram>, IOrderedQueryable<NdmAudiogram>> orderBy = null,
        Func<IQueryable<NdmAudiogram>, IIncludableQueryable<NdmAudiogram, object>> includes = null)
    {
        var dbSet = _context.Set<NdmAudiogram>();
        IQueryable<NdmAudiogram> query = dbSet;

        // https://stackoverflow.com/questions/46374252/how-to-write-repository-method-for-theninclude-in-ef-core-2
        if (includes != null) query = includes(query);
        if (filter != null) query = query.Where(filter);
        if (orderBy != null) query = orderBy(query);
        return query;
    }
}