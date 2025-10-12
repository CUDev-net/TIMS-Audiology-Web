using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IMarketingReferenceValidator
{
    Task<List<ValidationResult>> AddNew(MarketingReference marketingReference);
    Task<List<ValidationResult>> Update(MarketingReference marketingReference);
}

public class MarketingReferenceValidator : IMarketingReferenceValidator
{
    private readonly IMarketingReferenceUnitOfWork _marketingReferenceUnitOfWork;

    public MarketingReferenceValidator(IMarketingReferenceUnitOfWork marketingReferenceUnitOfWork)
    {
        _marketingReferenceUnitOfWork = marketingReferenceUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(MarketingReference marketingReference)
    {
        var validationResults = _ValidateBase(marketingReference);
        if (validationResults.Count == 0)
        {
            var existing = await _marketingReferenceUnitOfWork
                .GetMarketingReferences(a => a.Name == marketingReference.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(MarketingReference marketingReference)
    {
        var validationResults = _ValidateBase(marketingReference);
        if (validationResults.Count == 0)
        {
            var existing = await _marketingReferenceUnitOfWork
                .GetMarketingReferences(
                    a => a.Name == marketingReference.Name && a.Id != marketingReference.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(MarketingReference marketingReference)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(marketingReference.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}