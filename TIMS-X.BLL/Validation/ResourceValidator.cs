using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IResourceValidator
{
    Task<List<ValidationResult>> AddNew(Resource resource);
    Task<List<ValidationResult>> Update(Resource resource);
}

public class ResourceValidator : IResourceValidator
{
    private readonly IResourceUnitOfWork _resourceUnitOfWork;

    public ResourceValidator(IResourceUnitOfWork resourceUnitOfWork)
    {
        _resourceUnitOfWork = resourceUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(Resource resource)
    {
        var validationResults = _ValidateBase(resource);
        if (validationResults.Count == 0)
        {
            var existing = await _resourceUnitOfWork
                .GetResources(a => a.Name == resource.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(Resource resource)
    {
        var validationResults = _ValidateBase(resource);
        if (validationResults.Count == 0)
        {
            var existing = await _resourceUnitOfWork
                .GetResources(
                    a => a.Name == resource.Name && a.Id != resource.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(Resource resource)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(resource.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));

        return validationResults;
    }
}