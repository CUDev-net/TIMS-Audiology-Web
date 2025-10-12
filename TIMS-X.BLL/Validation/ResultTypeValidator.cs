using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IResultTypeValidator
{
    Task<List<ValidationResult>> AddNew(ResultType resultType);
    Task<List<ValidationResult>> Update(ResultType resultType);
}

public class ResultTypeValidator : IResultTypeValidator
{
    private readonly IResultTypeUnitOfWork _resultTypeUnitOfWork;

    public ResultTypeValidator(IResultTypeUnitOfWork resultTypeUnitOfWork)
    {
        _resultTypeUnitOfWork = resultTypeUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(ResultType resultType)
    {
        var validationResults = _ValidateBase(resultType);
        if (validationResults.Count == 0)
        {
            var existing = await _resultTypeUnitOfWork
                .GetResultTypes(a => a.Name == resultType.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(ResultType resultType)
    {
        var validationResults = _ValidateBase(resultType);
        if (validationResults.Count == 0)
        {
            var existing = await _resultTypeUnitOfWork
                .GetResultTypes(
                    a => a.Name == resultType.Name && a.Id != resultType.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(ResultType resultType)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(resultType.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}