using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IResultValidator
{
    Task<List<ValidationResult>> AddNew(Result result);
    Task<List<ValidationResult>> Update(Result result);
}

public class ResultValidator : IResultValidator
{
    private readonly IResultUnitOfWork _resultUnitOfWork;

    public ResultValidator(IResultUnitOfWork resultUnitOfWork)
    {
        _resultUnitOfWork = resultUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(Result result)
    {
        var validationResults = _ValidateBase(result);
        if (validationResults.Count == 0)
        {
            var existing = await _resultUnitOfWork
                .GetResults(a => a.Name == result.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(Result result)
    {
        var validationResults = _ValidateBase(result);
        if (validationResults.Count == 0)
        {
            var existing = await _resultUnitOfWork
                .GetResults(
                    a => a.Name == result.Name && a.Id != result.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(Result result)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(result.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}