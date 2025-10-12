using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IDiagnosisCodeValidator
{
    Task<List<ValidationResult>> AddNew(DiagnosisCode diagnosisCode);
    Task<List<ValidationResult>> Update(DiagnosisCode diagnosisCode);
}

public class DiagnosisCodeValidator : IDiagnosisCodeValidator
{
    private readonly IDiagnosisCodeCategoryUnitOfWork _diagnosisCodeCategoryUnitOfWork;
    private readonly IDiagnosisCodeUnitOfWork _diagnosisCodeUnitOfWork;

    public DiagnosisCodeValidator(IDiagnosisCodeUnitOfWork diagnosisCodeUnitOfWork,
        IDiagnosisCodeCategoryUnitOfWork diagnosisCodeCategoryUnitOfWork)
    {
        _diagnosisCodeUnitOfWork = diagnosisCodeUnitOfWork;
        _diagnosisCodeCategoryUnitOfWork = diagnosisCodeCategoryUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(DiagnosisCode diagnosisCode)
    {
        var validationResults = _ValidateBase(diagnosisCode);
        if (validationResults.Count == 0)
        {
            var existing = await _diagnosisCodeUnitOfWork
                .GetDiagnosisCodes(a => a.Name == diagnosisCode.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(DiagnosisCode diagnosisCode)
    {
        var validationResults = _ValidateBase(diagnosisCode);
        if (validationResults.Count == 0)
        {
            var existing = await _diagnosisCodeUnitOfWork
                .GetDiagnosisCodes(
                    a => a.Name == diagnosisCode.Name && a.Id != diagnosisCode.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(DiagnosisCode diagnosisCode)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(diagnosisCode.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        if (_diagnosisCodeCategoryUnitOfWork.GetDiagnosisCodeCategory(diagnosisCode.CategoryId).Result == null)
            validationResults.Add(new ValidationResult("Category must exist", Severity.Error));
        return validationResults;
    }
}