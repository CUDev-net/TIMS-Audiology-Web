using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IAdjustmentTypeValidator
{
    Task<List<ValidationResult>> AddNew(AdjustmentType adjustmentType);
    Task<List<ValidationResult>> Update(AdjustmentType adjustmentType);
}

public class AdjustmentTypeValidator : IAdjustmentTypeValidator
{
    private readonly IAdjustmentTypeUnitOfWork _adjustmentTypeUnitOfWork;

    public AdjustmentTypeValidator(IAdjustmentTypeUnitOfWork adjustmentTypeUnitOfWork)
    {
        _adjustmentTypeUnitOfWork = adjustmentTypeUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(AdjustmentType adjustmentType)
    {
        var validationResults = _ValidateBase(adjustmentType);
        if (validationResults.Count == 0)
        {
            var existing = await _adjustmentTypeUnitOfWork
                .GetAdjustmentTypes(a => a.Name == adjustmentType.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(AdjustmentType adjustmentType)
    {
        var validationResults = _ValidateBase(adjustmentType);
        if (validationResults.Count == 0)
        {
            var existing = await _adjustmentTypeUnitOfWork
                .GetAdjustmentTypes(
                    a => a.Name == adjustmentType.Name && a.Id != adjustmentType.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(AdjustmentType adjustmentType)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(adjustmentType.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}