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

public interface IScriptedNoteCategoryUnitOfWork : IUnitOfWork
{
    Task<ScriptedNoteCategory> GetScriptedNoteCategory(int id);

    Task<List<ScriptedNoteCategory>> GetScriptedNoteCategories(
        Expression<Func<ScriptedNoteCategory, bool>> filter = null,
        Func<IQueryable<ScriptedNoteCategory>, IOrderedQueryable<ScriptedNoteCategory>> orderBy = null,
        Func<IQueryable<ScriptedNoteCategory>, IIncludableQueryable<ScriptedNoteCategory, object>> includes = null);
}

public class ScriptedNoteCategoryUnitOfWork : UnitOfWorkBase, IScriptedNoteCategoryUnitOfWork
{
    public ScriptedNoteCategoryUnitOfWork(TimsContext context, IHttpContextAccessor httpContextAccessor) : base(context,
        httpContextAccessor)
    {
    }

    protected override string TableName => nameof(ScriptedNoteCategory);

    public async Task<ScriptedNoteCategory> GetScriptedNoteCategory(int id)
    {
        return await Single<ScriptedNoteCategory>(u => u.Id == id);
    }

    public async Task<List<ScriptedNoteCategory>> GetScriptedNoteCategories(
        Expression<Func<ScriptedNoteCategory, bool>> filter = null,
        Func<IQueryable<ScriptedNoteCategory>, IOrderedQueryable<ScriptedNoteCategory>> orderBy = null,
        Func<IQueryable<ScriptedNoteCategory>, IIncludableQueryable<ScriptedNoteCategory, object>> includes = null)
    {
        return await Get(filter, orderBy, includes).ToListAsync();
    }
}