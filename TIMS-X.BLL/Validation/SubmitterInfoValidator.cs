using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface ISubmitterInfoValidator
{
    Task<List<ValidationResult>> AddNew(SubmitterInfo submitterInfo);
    Task<List<ValidationResult>> Update(SubmitterInfo submitterInfo);
}

public class SubmitterInfoValidator : ISubmitterInfoValidator
{
    private readonly ISubmitterInfoUnitOfWork _submitterInfoUnitOfWork;

    public SubmitterInfoValidator(ISubmitterInfoUnitOfWork submitterInfoUnitOfWork)
    {
        _submitterInfoUnitOfWork = submitterInfoUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(SubmitterInfo submitterInfo)
    {
        var validationResults = _ValidateBase(submitterInfo);
        if (validationResults.Count == 0)
        {
            var existing = await _submitterInfoUnitOfWork
                .GetSubmitterInfos(a => a.Name == submitterInfo.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(SubmitterInfo submitterInfo)
    {
        var validationResults = _ValidateBase(submitterInfo);
        if (validationResults.Count == 0)
        {
            var existing = await _submitterInfoUnitOfWork
                .GetSubmitterInfos(
                    a => a.Name == submitterInfo.Name && a.Id != submitterInfo.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(SubmitterInfo submitterInfo)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(submitterInfo.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}