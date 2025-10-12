using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface ICountyValidator
{
    Task<List<ValidationResult>> AddNew(County county);
    Task<List<ValidationResult>> Update(County county);
}

public class CountyValidator : ICountyValidator
{
    private readonly ICountyUnitOfWork _countyUnitOfWork;

    public CountyValidator(ICountyUnitOfWork countyUnitOfWork)
    {
        _countyUnitOfWork = countyUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(County county)
    {
        var validationResults = _ValidateBase(county);
        if (validationResults.Count == 0)
        {
            var existing = await _countyUnitOfWork
                .GetCounties(a => a.Name == county.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(County county)
    {
        var validationResults = _ValidateBase(county);
        if (validationResults.Count == 0)
        {
            var existing = await _countyUnitOfWork
                .GetCounties(
                    a => a.Name == county.Name && a.Id != county.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(County county)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(county.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}