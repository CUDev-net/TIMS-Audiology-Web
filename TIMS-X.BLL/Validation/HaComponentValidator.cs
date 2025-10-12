using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IHaComponentValidator
{
    Task<List<ValidationResult>> AddNew(HaComponent haComponent);
    Task<List<ValidationResult>> Update(HaComponent haComponent);
}

public class HaComponentValidator : IHaComponentValidator
{
    private readonly IHaComponentUnitOfWork _haComponentUnitOfWork;

    public HaComponentValidator(IHaComponentUnitOfWork haComponentUnitOfWork)
    {
        _haComponentUnitOfWork = haComponentUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(HaComponent haComponent)
    {
        var validationResults = _ValidateBase(haComponent);
        if (validationResults.Count == 0)
        {
            var existing = await _haComponentUnitOfWork
                .GetHaComponents(a => a.Name == haComponent.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(HaComponent haComponent)
    {
        var validationResults = _ValidateBase(haComponent);
        if (validationResults.Count == 0)
        {
            var existing = await _haComponentUnitOfWork
                .GetHaComponents(
                    a => a.Name == haComponent.Name && a.Id != haComponent.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(HaComponent haComponent)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(haComponent.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));

        return validationResults;
    }
}