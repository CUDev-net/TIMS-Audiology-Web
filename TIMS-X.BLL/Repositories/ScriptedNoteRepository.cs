using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IScriptedNoteRepository
{
    Task<ScriptedNote> Add(ScriptedNote site);
    void Delete(int id);
    Task<ScriptedNote> Get(int id);
    Task<List<ScriptedNote>> GetAll(bool includeInactive);
    Task<ScriptedNote> Update(ScriptedNote scriptedNote);
}

public class ScriptedNoteRepository : IScriptedNoteRepository
{
    private readonly IScriptedNoteUnitOfWork _scriptedNoteUnitOfWork;

    public ScriptedNoteRepository(IScriptedNoteUnitOfWork scriptedNoteUnitOfWork)
    {
        _scriptedNoteUnitOfWork = scriptedNoteUnitOfWork;
    }

    public async Task<ScriptedNote> Add(ScriptedNote scriptedNote)
    {
        return await _scriptedNoteUnitOfWork.Add(scriptedNote);
    }

    public async Task<ScriptedNote> Get(int id)
    {
        return await _scriptedNoteUnitOfWork.GetScriptedNote(id);
    }

    public async Task<List<ScriptedNote>> GetAll(bool includeInactive)
    {
        return await _scriptedNoteUnitOfWork.GetScriptedNotes(x => includeInactive || !x.Inactive);
    }

    public async Task<ScriptedNote> Update(ScriptedNote scriptedNote)
    {
        return await _scriptedNoteUnitOfWork.Update(scriptedNote);
    }

    public void Delete(int id)
    {
        _scriptedNoteUnitOfWork.Delete(id);
    }
}