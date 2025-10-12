using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface ISiteValidator
{
    Task<List<ValidationResult>> AddNew(Site site);
    Task<List<ValidationResult>> Update(Site site);
}

public class SiteValidator : ISiteValidator
{
    private readonly ISiteUnitOfWork _siteUnitOfWork;

    public SiteValidator(ISiteUnitOfWork siteUnitOfWork)
    {
        _siteUnitOfWork = siteUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(Site site)
    {
        var validationResults = _ValidateBase(site);
        if (validationResults.Count == 0)
        {
            var existing = await _siteUnitOfWork
                .GetSites(a => a.Name == site.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(Site site)
    {
        var validationResults = _ValidateBase(site);
        if (validationResults.Count == 0)
        {
            var existing = await _siteUnitOfWork
                .GetSites(
                    a => a.Name == site.Name && a.Id != site.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(Site site)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(site.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));

        return validationResults;
    }
}