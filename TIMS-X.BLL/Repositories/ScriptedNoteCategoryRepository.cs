using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IScriptedNoteCategoryRepository
{
    Task<ScriptedNoteCategory> Add(ScriptedNoteCategory scriptedNoteCategory);
    void Delete(int id);
    Task<ScriptedNoteCategory> Get(int id);
    Task<List<ScriptedNoteCategory>> GetAll(bool includeInactive);
    Task<ScriptedNoteCategory> Update(ScriptedNoteCategory scriptedNoteCategory);
}

public class ScriptedNoteCategoryRepository : IScriptedNoteCategoryRepository
{
    private readonly IScriptedNoteCategoryUnitOfWork _scriptedNoteCategoryUnitOfWork;

    public ScriptedNoteCategoryRepository(IScriptedNoteCategoryUnitOfWork scriptedNoteCategoryUnitOfWork)
    {
        _scriptedNoteCategoryUnitOfWork = scriptedNoteCategoryUnitOfWork;
    }

    public async Task<ScriptedNoteCategory> Add(ScriptedNoteCategory scriptedNoteCategory)
    {
        return await _scriptedNoteCategoryUnitOfWork.Add(scriptedNoteCategory);
    }

    public async Task<ScriptedNoteCategory> Get(int id)
    {
        return await _scriptedNoteCategoryUnitOfWork.GetScriptedNoteCategory(id);
    }

    public async Task<List<ScriptedNoteCategory>> GetAll(bool includeInactive)
    {
        return await _scriptedNoteCategoryUnitOfWork.GetScriptedNoteCategories(x => includeInactive || !x.Inactive);
    }

    public async Task<ScriptedNoteCategory> Update(ScriptedNoteCategory scriptedNoteCategory)
    {
        return await _scriptedNoteCategoryUnitOfWork.Update(scriptedNoteCategory);
    }

    public void Delete(int id)
    {
        _scriptedNoteCategoryUnitOfWork.Delete(id);
    }
}