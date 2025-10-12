using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IScriptedNoteValidator
{
    Task<List<ValidationResult>> AddNew(ScriptedNote scriptedNote);
    Task<List<ValidationResult>> Update(ScriptedNote scriptedNote);
}

public class ScriptedNoteValidator : IScriptedNoteValidator
{
    private readonly IScriptedNoteCategoryUnitOfWork _scriptedNoteCategoryUnitOfWork;
    private readonly IScriptedNoteUnitOfWork _scriptedNoteUnitOfWork;

    public ScriptedNoteValidator(IScriptedNoteUnitOfWork scriptedNoteUnitOfWork,
        IScriptedNoteCategoryUnitOfWork scriptedNoteCategoryUnitOfWork)
    {
        _scriptedNoteUnitOfWork = scriptedNoteUnitOfWork;
        _scriptedNoteCategoryUnitOfWork = scriptedNoteCategoryUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(ScriptedNote scriptedNote)
    {
        var validationResults = _ValidateBase(scriptedNote);
        if (validationResults.Count == 0)
        {
            var existing = await _scriptedNoteUnitOfWork
                .GetScriptedNotes(a => a.Name == scriptedNote.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(ScriptedNote scriptedNote)
    {
        var validationResults = _ValidateBase(scriptedNote);
        if (validationResults.Count == 0)
        {
            var existing = await _scriptedNoteUnitOfWork
                .GetScriptedNotes(
                    a => a.Name == scriptedNote.Name && a.Id != scriptedNote.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(ScriptedNote scriptedNote)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(scriptedNote.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        if (_scriptedNoteCategoryUnitOfWork.GetScriptedNoteCategory(scriptedNote.CategoryId).Result == null)
            validationResults.Add(new ValidationResult("Category must exist", Severity.Error));
        return validationResults;
    }
}