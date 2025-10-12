using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface ISalutationValidator
{
    Task<List<ValidationResult>> AddNew(Salutation salutation);
    Task<List<ValidationResult>> Update(Salutation salutation);
}

public class SalutationValidator : ISalutationValidator
{
    private readonly ISalutationUnitOfWork _salutationUnitOfWork;

    public SalutationValidator(ISalutationUnitOfWork salutationUnitOfWork)
    {
        _salutationUnitOfWork = salutationUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(Salutation salutation)
    {
        var validationResults = _ValidateBase(salutation);
        if (validationResults.Count == 0)
        {
            var existing = await _salutationUnitOfWork
                .GetSalutations(a => a.Name == salutation.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(Salutation salutation)
    {
        var validationResults = _ValidateBase(salutation);
        if (validationResults.Count == 0)
        {
            var existing = await _salutationUnitOfWork
                .GetSalutations(
                    a => a.Name == salutation.Name && a.Id != salutation.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(Salutation salutation)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(salutation.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));

        return validationResults;
    }
}