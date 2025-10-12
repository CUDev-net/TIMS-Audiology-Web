using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface ISexValidator
{
    Task<List<ValidationResult>> AddNew(Sex sex);
    Task<List<ValidationResult>> Update(Sex sex);
}

public class SexValidator : ISexValidator
{
    private readonly ISexUnitOfWork _sexUnitOfWork;

    public SexValidator(ISexUnitOfWork sexUnitOfWork)
    {
        _sexUnitOfWork = sexUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(Sex sex)
    {
        var validationResults = _ValidateBase(sex);
        if (validationResults.Count == 0)
        {
            var existing = await _sexUnitOfWork
                .GetSexes(a => a.Name == sex.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(Sex sex)
    {
        var validationResults = _ValidateBase(sex);
        if (validationResults.Count == 0)
        {
            var existing = await _sexUnitOfWork
                .GetSexes(
                    a => a.Name == sex.Name && a.Id != sex.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(Sex sex)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(sex.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}