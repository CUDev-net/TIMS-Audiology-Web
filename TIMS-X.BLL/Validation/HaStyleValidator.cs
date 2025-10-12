using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IHaStyleValidator
{
    Task<List<ValidationResult>> AddNew(HaStyle haStyle);
    Task<List<ValidationResult>> Update(HaStyle haStyle);
}

public class HaStyleValidator : IHaStyleValidator
{
    private readonly IHaStyleUnitOfWork _haStyleUnitOfWork;

    public HaStyleValidator(IHaStyleUnitOfWork haStyleUnitOfWork)
    {
        _haStyleUnitOfWork = haStyleUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(HaStyle haStyle)
    {
        var validationResults = _ValidateBase(haStyle);
        if (validationResults.Count == 0)
        {
            var existing = await _haStyleUnitOfWork
                .GetHaStyles(a => a.Name == haStyle.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(HaStyle haStyle)
    {
        var validationResults = _ValidateBase(haStyle);
        if (validationResults.Count == 0)
        {
            var existing = await _haStyleUnitOfWork
                .GetHaStyles(
                    a => a.Name == haStyle.Name && a.Id != haStyle.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(HaStyle haStyle)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(haStyle.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));

        return validationResults;
    }
}