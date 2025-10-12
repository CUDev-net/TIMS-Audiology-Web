using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IMarketingCategoryValidator
{
    Task<List<ValidationResult>> AddNew(MarketingReferenceCategory marketingReferenceCategory);
    Task<List<ValidationResult>> Update(MarketingReferenceCategory marketingReferenceCategory);
}

public class MarketingCategoryValidator : IMarketingCategoryValidator
{
    private readonly IMarketingCategoryUnitOfWork _marketingCategoryUnitOfWork;

    public MarketingCategoryValidator(IMarketingCategoryUnitOfWork marketingCategoryUnitOfWork)
    {
        _marketingCategoryUnitOfWork = marketingCategoryUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(MarketingReferenceCategory marketingCategory)
    {
        var validationResults = _ValidateBase(marketingCategory);
        if (validationResults.Count == 0)
        {
            var existing = await _marketingCategoryUnitOfWork
                .GetMarketingCategories(a => a.Name == marketingCategory.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(MarketingReferenceCategory marketingCategory)
    {
        var validationResults = _ValidateBase(marketingCategory);
        if (validationResults.Count == 0)
        {
            var existing = await _marketingCategoryUnitOfWork
                .GetMarketingCategories(
                    a => a.Name == marketingCategory.Name && a.Id != marketingCategory.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(MarketingReferenceCategory marketingCategory)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(marketingCategory.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}