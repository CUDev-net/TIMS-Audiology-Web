using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IDiagnosisCodeCategoryValidator
{
    Task<List<ValidationResult>> AddNew(DiagnosisCodeCategory diagnosisCodeCategory);
    Task<List<ValidationResult>> Update(DiagnosisCodeCategory diagnosisCodeCategory);
}

public class DiagnosisCodeCategoryValidator : IDiagnosisCodeCategoryValidator
{
    private readonly IDiagnosisCodeCategoryUnitOfWork _diagnosisCodeCategoryUnitOfWork;

    public DiagnosisCodeCategoryValidator(IDiagnosisCodeCategoryUnitOfWork diagnosisCodeCategoryUnitOfWork)
    {
        _diagnosisCodeCategoryUnitOfWork = diagnosisCodeCategoryUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(DiagnosisCodeCategory diagnosisCodeCategory)
    {
        var validationResults = _ValidateBase(diagnosisCodeCategory);
        if (validationResults.Count == 0)
        {
            var existing = await _diagnosisCodeCategoryUnitOfWork
                .GetDiagnosisCodeCategories(a => a.Name == diagnosisCodeCategory.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(DiagnosisCodeCategory diagnosisCodeCategory)
    {
        var validationResults = _ValidateBase(diagnosisCodeCategory);
        if (validationResults.Count == 0)
        {
            var existing = await _diagnosisCodeCategoryUnitOfWork
                .GetDiagnosisCodeCategories(
                    a => a.Name == diagnosisCodeCategory.Name && a.Id != diagnosisCodeCategory.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(DiagnosisCodeCategory diagnosisCodeCategory)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(diagnosisCodeCategory.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}