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

public interface ICommunicationRestrictionUnitOfWork : IUnitOfWork
{
    Task<CommunicationRestriction> GetCommunicationRestriction(int id);

    Task<List<CommunicationRestriction>> GetCommunicationRestrictions(Expression<Func<CommunicationRestriction, bool>> filter = null,
        Func<IQueryable<CommunicationRestriction>, IOrderedQueryable<CommunicationRestriction>> orderBy = null,
        Func<IQueryable<CommunicationRestriction>, IIncludableQueryable<CommunicationRestriction, object>> includes = null);
}

public class CommunicationRestrictionUnitOfWork : UnitOfWorkBase, ICommunicationRestrictionUnitOfWork
{
    public CommunicationRestrictionUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "Restriction";

    public async Task<CommunicationRestriction> GetCommunicationRestriction(int id)
    {
        return await Single<CommunicationRestriction>(u => u.Id == id);
    }

    public async Task<List<CommunicationRestriction>> GetCommunicationRestrictions(Expression<Func<CommunicationRestriction, bool>> filter = null,
        Func<IQueryable<CommunicationRestriction>, IOrderedQueryable<CommunicationRestriction>> orderBy = null,
        Func<IQueryable<CommunicationRestriction>, IIncludableQueryable<CommunicationRestriction, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}