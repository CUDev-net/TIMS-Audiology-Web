using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface ICptCodeCategoryValidator
{
    Task<List<ValidationResult>> AddNew(CptCodeCategory cptCodeCategory);
    Task<List<ValidationResult>> Update(CptCodeCategory cptCodeCategory);
}

public class CptCodeCategoryValidator : ICptCodeCategoryValidator
{
    private readonly ICptCodeCategoryUnitOfWork _cptCodeCategoryUnitOfWork;

    public CptCodeCategoryValidator(ICptCodeCategoryUnitOfWork cptCodeCategoryUnitOfWork)
    {
        _cptCodeCategoryUnitOfWork = cptCodeCategoryUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(CptCodeCategory cptCodeCategory)
    {
        var validationResults = _ValidateBase(cptCodeCategory);
        if (validationResults.Count == 0)
        {
            var existing = await _cptCodeCategoryUnitOfWork
                .GetCptCodeCategories(a => a.Name == cptCodeCategory.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(CptCodeCategory cptCodeCategory)
    {
        var validationResults = _ValidateBase(cptCodeCategory);
        if (validationResults.Count == 0)
        {
            var existing = await _cptCodeCategoryUnitOfWork
                .GetCptCodeCategories(
                    a => a.Name == cptCodeCategory.Name && a.Id != cptCodeCategory.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(CptCodeCategory cptCodeCategory)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(cptCodeCategory.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}