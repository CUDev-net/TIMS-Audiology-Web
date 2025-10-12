using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IOutsideFacilityValidator
{
    Task<List<ValidationResult>> AddNew(OutsideFacility outsideFacility);
    Task<List<ValidationResult>> Update(OutsideFacility outsideFacility);
}

public class OutsideFacilityValidator : IOutsideFacilityValidator
{
    private readonly IOutsideFacilityUnitOfWork _outsideFacilityUnitOfWork;

    public OutsideFacilityValidator(IOutsideFacilityUnitOfWork outsideFacilityUnitOfWork)
    {
        _outsideFacilityUnitOfWork = outsideFacilityUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(OutsideFacility outsideFacility)
    {
        var validationResults = _ValidateBase(outsideFacility);
        if (validationResults.Count == 0)
        {
            var existing = await _outsideFacilityUnitOfWork
                .GetOutsideFacilities(a => a.Name == outsideFacility.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(OutsideFacility outsideFacility)
    {
        var validationResults = _ValidateBase(outsideFacility);
        if (validationResults.Count == 0)
        {
            var existing = await _outsideFacilityUnitOfWork
                .GetOutsideFacilities(
                    a => a.Name == outsideFacility.Name && a.Id != outsideFacility.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(OutsideFacility outsideFacility)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(outsideFacility.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}