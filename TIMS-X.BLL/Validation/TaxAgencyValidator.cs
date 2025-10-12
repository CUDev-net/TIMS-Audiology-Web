using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface ITaxAgencyValidator
{
    Task<List<ValidationResult>> AddNew(TaxAgency taxAgency);
    Task<List<ValidationResult>> Update(TaxAgency taxAgency);
}

public class TaxAgencyValidator : ITaxAgencyValidator
{
    private readonly ITaxAgencyUnitOfWork _taxAgencyUnitOfWork;

    public TaxAgencyValidator(ITaxAgencyUnitOfWork taxAgencyUnitOfWork)
    {
        _taxAgencyUnitOfWork = taxAgencyUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(TaxAgency taxAgency)
    {
        var validationResults = _ValidateBase(taxAgency);
        if (validationResults.Count == 0)
        {
            var existing = await _taxAgencyUnitOfWork
                .GetTaxAgencies(a => a.Name == taxAgency.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(TaxAgency taxAgency)
    {
        var validationResults = _ValidateBase(taxAgency);
        if (validationResults.Count == 0)
        {
            var existing = await _taxAgencyUnitOfWork
                .GetTaxAgencies(
                    a => a.Name == taxAgency.Name && a.Id != taxAgency.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(TaxAgency taxAgency)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(taxAgency.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}