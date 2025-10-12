using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IHaTypeValidator
{
    Task<List<ValidationResult>> AddNew(HaType haType);
    Task<List<ValidationResult>> Update(HaType haType);
}

public class HaTypeValidator : IHaTypeValidator
{
    private readonly IHaTypeUnitOfWork _haTypeUnitOfWork;

    public HaTypeValidator(IHaTypeUnitOfWork haTypeUnitOfWork)
    {
        _haTypeUnitOfWork = haTypeUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(HaType haType)
    {
        var validationResults = _ValidateBase(haType);
        if (validationResults.Count == 0)
        {
            var existing = await _haTypeUnitOfWork
                .GetHaTypes(a => a.Name == haType.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(HaType haType)
    {
        var validationResults = _ValidateBase(haType);
        if (validationResults.Count == 0)
        {
            var existing = await _haTypeUnitOfWork
                .GetHaTypes(
                    a => a.Name == haType.Name && a.Id != haType.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(HaType haType)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(haType.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));

        return validationResults;
    }
}