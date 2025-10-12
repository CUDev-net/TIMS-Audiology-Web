using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface ITaxGroupValidator
{
    Task<List<ValidationResult>> AddNew(TaxGroup taxGroup);
    Task<List<ValidationResult>> Update(TaxGroup taxGroup);
}

public class TaxGroupValidator : ITaxGroupValidator
{
    private readonly ITaxGroupUnitOfWork _taxGroupUnitOfWork;

    public TaxGroupValidator(ITaxGroupUnitOfWork taxGroupUnitOfWork)
    {
        _taxGroupUnitOfWork = taxGroupUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(TaxGroup taxGroup)
    {
        var validationResults = _ValidateBase(taxGroup);
        if (validationResults.Count == 0)
        {
            var existing = await _taxGroupUnitOfWork
                .GetTaxGroups(a => a.Name == taxGroup.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(TaxGroup taxGroup)
    {
        var validationResults = _ValidateBase(taxGroup);
        if (validationResults.Count == 0)
        {
            var existing = await _taxGroupUnitOfWork
                .GetTaxGroups(
                    a => a.Name == taxGroup.Name && a.Id != taxGroup.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(TaxGroup taxGroup)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(taxGroup.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}