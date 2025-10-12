using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IScriptedNoteCategoryValidator
{
    Task<List<ValidationResult>> AddNew(ScriptedNoteCategory scriptedNoteCategory);
    Task<List<ValidationResult>> Update(ScriptedNoteCategory scriptedNoteCategory);
}

public class ScriptedNoteCategoryValidator : IScriptedNoteCategoryValidator
{
    private readonly IScriptedNoteCategoryUnitOfWork _scriptedNoteCategoryUnitOfWork;

    public ScriptedNoteCategoryValidator(IScriptedNoteCategoryUnitOfWork scriptedNoteCategoryUnitOfWork)
    {
        _scriptedNoteCategoryUnitOfWork = scriptedNoteCategoryUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(ScriptedNoteCategory scriptedNoteCategory)
    {
        var validationResults = _ValidateBase(scriptedNoteCategory);
        if (validationResults.Count == 0)
        {
            var existing = await _scriptedNoteCategoryUnitOfWork
                .GetScriptedNoteCategories(a => a.Name == scriptedNoteCategory.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(ScriptedNoteCategory scriptedNoteCategory)
    {
        var validationResults = _ValidateBase(scriptedNoteCategory);
        if (validationResults.Count == 0)
        {
            var existing = await _scriptedNoteCategoryUnitOfWork
                .GetScriptedNoteCategories(
                    a => a.Name == scriptedNoteCategory.Name && a.Id != scriptedNoteCategory.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(ScriptedNoteCategory scriptedNoteCategory)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(scriptedNoteCategory.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}