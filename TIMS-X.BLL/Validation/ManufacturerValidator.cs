using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IManufacturerValidator
{
    Task<List<ValidationResult>> AddNew(Manufacturer manufacturer);
    Task<List<ValidationResult>> Update(Manufacturer manufacturer);
}

public class ManufacturerValidator : IManufacturerValidator
{
    private readonly IManufacturerUnitOfWork _manufacturerUnitOfWork;

    public ManufacturerValidator(IManufacturerUnitOfWork manufacturerUnitOfWork)
    {
        _manufacturerUnitOfWork = manufacturerUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(Manufacturer manufacturer)
    {
        var validationResults = _ValidateBase(manufacturer);
        if (validationResults.Count == 0)
        {
            var existing = await _manufacturerUnitOfWork
                .GetManufacturers(a => a.Name == manufacturer.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(Manufacturer manufacturer)
    {
        var validationResults = _ValidateBase(manufacturer);
        if (validationResults.Count == 0)
        {
            var existing = await _manufacturerUnitOfWork
                .GetManufacturers(
                    a => a.Name == manufacturer.Name && a.Id != manufacturer.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(Manufacturer manufacturer)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(manufacturer.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}