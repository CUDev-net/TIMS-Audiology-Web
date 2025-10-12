using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IModifierValidator
{
    Task<List<ValidationResult>> AddNew(Modifier modifier);
    Task<List<ValidationResult>> Update(Modifier modifier);
}

public class ModifierValidator : IModifierValidator
{
    private readonly IModifierUnitOfWork _modifierUnitOfWork;

    public ModifierValidator(IModifierUnitOfWork modifierUnitOfWork)
    {
        _modifierUnitOfWork = modifierUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(Modifier modifier)
    {
        var validationResults = _ValidateBase(modifier);
        if (validationResults.Count == 0)
        {
            var existing = await _modifierUnitOfWork
                .GetModifiers(a => a.Name == modifier.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(Modifier modifier)
    {
        var validationResults = _ValidateBase(modifier);
        if (validationResults.Count == 0)
        {
            var existing = await _modifierUnitOfWork
                .GetModifiers(
                    a => a.Name == modifier.Name && a.Id != modifier.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(Modifier modifier)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(modifier.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}