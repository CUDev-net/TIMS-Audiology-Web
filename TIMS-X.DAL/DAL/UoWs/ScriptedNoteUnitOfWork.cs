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

public interface IScriptedNoteUnitOfWork : IUnitOfWork
{
    Task<ScriptedNote> GetScriptedNote(int id);

    Task<List<ScriptedNote>> GetScriptedNotes(Expression<Func<ScriptedNote, bool>> filter = null,
        Func<IQueryable<ScriptedNote>, IOrderedQueryable<ScriptedNote>> orderBy = null,
        Func<IQueryable<ScriptedNote>, IIncludableQueryable<ScriptedNote, object>> includes = null);
}

public class ScriptedNoteUnitOfWork : UnitOfWorkBase, IScriptedNoteUnitOfWork
{
    public ScriptedNoteUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(ScriptedNote);

    public async Task<ScriptedNote> GetScriptedNote(int id)
    {
        return await Single<ScriptedNote>(u => u.Id == id);
    }

    public async Task<List<ScriptedNote>> GetScriptedNotes(Expression<Func<ScriptedNote, bool>> filter = null,
        Func<IQueryable<ScriptedNote>, IOrderedQueryable<ScriptedNote>> orderBy = null,
        Func<IQueryable<ScriptedNote>, IIncludableQueryable<ScriptedNote, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}