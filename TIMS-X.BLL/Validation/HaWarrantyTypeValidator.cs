using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IHaWarrantyTypeValidator
{
    Task<List<ValidationResult>> AddNew(HaWarrantyType haWarrantyType);
    Task<List<ValidationResult>> Update(HaWarrantyType haWarrantyType);
}

public class HaWarrantyTypeValidator : IHaWarrantyTypeValidator
{
    private readonly IHaWarrantyTypeUnitOfWork _haWarrantyTypeUnitOfWork;

    public HaWarrantyTypeValidator(IHaWarrantyTypeUnitOfWork haWarrantyTypeUnitOfWork)
    {
        _haWarrantyTypeUnitOfWork = haWarrantyTypeUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(HaWarrantyType haWarrantyType)
    {
        var validationResults = _ValidateBase(haWarrantyType);
        if (validationResults.Count == 0)
        {
            var existing = await _haWarrantyTypeUnitOfWork
                .GetHaWarrantyTypes(a => a.Name == haWarrantyType.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(HaWarrantyType haWarrantyType)
    {
        var validationResults = _ValidateBase(haWarrantyType);
        if (validationResults.Count == 0)
        {
            var existing = await _haWarrantyTypeUnitOfWork
                .GetHaWarrantyTypes(
                    a => a.Name == haWarrantyType.Name && a.Id != haWarrantyType.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(HaWarrantyType haWarrantyType)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(haWarrantyType.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));

        return validationResults;
    }
}