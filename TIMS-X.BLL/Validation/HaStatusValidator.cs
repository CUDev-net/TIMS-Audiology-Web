using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IHaStatusValidator
{
    Task<List<ValidationResult>> AddNew(HaStatus haStatus);
    Task<List<ValidationResult>> Update(HaStatus haStatus);
}

public class HaStatusValidator : IHaStatusValidator
{
    private readonly IHaStatusUnitOfWork _haStatusUnitOfWork;

    public HaStatusValidator(IHaStatusUnitOfWork haStatusUnitOfWork)
    {
        _haStatusUnitOfWork = haStatusUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(HaStatus haStatus)
    {
        var validationResults = _ValidateBase(haStatus);
        if (validationResults.Count == 0)
        {
            var existing = await _haStatusUnitOfWork
                .GetHaStatuses(a => a.Name == haStatus.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(HaStatus haStatus)
    {
        var validationResults = _ValidateBase(haStatus);
        if (validationResults.Count == 0)
        {
            var existing = await _haStatusUnitOfWork
                .GetHaStatuses(
                    a => a.Name == haStatus.Name && a.Id != haStatus.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(HaStatus haStatus)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(haStatus.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));

        return validationResults;
    }
}