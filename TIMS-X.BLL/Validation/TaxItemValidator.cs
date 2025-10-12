using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface ITaxItemValidator
{
    Task<List<ValidationResult>> AddNew(TaxItem taxItem);
    Task<List<ValidationResult>> Update(TaxItem taxItem);
}

public class TaxItemValidator : ITaxItemValidator
{
    private readonly ITaxAgencyUnitOfWork _taxAgencyUnitOfWork;
    private readonly ITaxItemUnitOfWork _taxItemUnitOfWork;

    public TaxItemValidator(ITaxItemUnitOfWork taxItemUnitOfWork,
        ITaxAgencyUnitOfWork taxAgencyUnitOfWork)
    {
        _taxItemUnitOfWork = taxItemUnitOfWork;
        _taxAgencyUnitOfWork = taxAgencyUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(TaxItem taxItem)
    {
        var validationResults = _ValidateBase(taxItem);
        if (validationResults.Count == 0)
        {
            var existing = await _taxItemUnitOfWork
                .GetTaxItems(a => a.Name == taxItem.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(TaxItem taxItem)
    {
        var validationResults = _ValidateBase(taxItem);
        if (validationResults.Count == 0)
        {
            var existing = await _taxItemUnitOfWork
                .GetTaxItems(
                    a => a.Name == taxItem.Name && a.Id != taxItem.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(TaxItem taxItem)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(taxItem.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        if (taxItem.AgencyId > 0)
            if (_taxAgencyUnitOfWork.GetTaxAgency(taxItem.AgencyId).Result == null)
                validationResults.Add(new ValidationResult("Tax Agency must exist", Severity.Error));
        return validationResults;
    }
}