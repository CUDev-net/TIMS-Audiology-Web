using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.UoWs;

public interface IPosDocumentUnitOfWork
{
    Task<PosDocument> GetPosDocument(int id,
        Func<IQueryable<PosDocument>, IIncludableQueryable<PosDocument, object>> includes = null);

    IQueryable<PosDocument> GetPosDocuments(Expression<Func<PosDocument, bool>> filter = null,
        Func<IQueryable<PosDocument>, IOrderedQueryable<PosDocument>> orderBy = null,
        Func<IQueryable<PosDocument>, IIncludableQueryable<PosDocument, object>> includes = null);
}

public class PosDocumentUnitOfWork : UnitOfWorkBase, IPosDocumentUnitOfWork
{
    public PosDocumentUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => "POSDocument";

    public async Task<PosDocument> GetPosDocument(int id,
        Func<IQueryable<PosDocument>, IIncludableQueryable<PosDocument, object>> includes = null)
    {
        return await Single(u => u.Id == id, includes);
    }

    public IQueryable<PosDocument> GetPosDocuments(Expression<Func<PosDocument, bool>> filter = null,
        Func<IQueryable<PosDocument>, IOrderedQueryable<PosDocument>> orderBy = null,
        Func<IQueryable<PosDocument>, IIncludableQueryable<PosDocument, object>> includes = null)
    {
        return Get(filter, orderBy, includes);
    }
}