using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IHistoryTypeValidator
{
    Task<List<ValidationResult>> AddNew(HistoryType historyType);
    Task<List<ValidationResult>> Update(HistoryType historyType);
}

public class HistoryTypeValidator : IHistoryTypeValidator
{
    private readonly IHistoryTypeUnitOfWork _historyTypeUnitOfWork;

    public HistoryTypeValidator(IHistoryTypeUnitOfWork historyTypeUnitOfWork)
    {
        _historyTypeUnitOfWork = historyTypeUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(HistoryType historyType)
    {
        var validationResults = _ValidateBase(historyType);
        if (validationResults.Count == 0)
        {
            var existing = await _historyTypeUnitOfWork
                .GetHistoryTypes(a => a.Name == historyType.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(HistoryType historyType)
    {
        var validationResults = _ValidateBase(historyType);
        if (validationResults.Count == 0)
        {
            var existing = await _historyTypeUnitOfWork
                .GetHistoryTypes(
                    a => a.Name == historyType.Name && a.Id != historyType.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(HistoryType historyType)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(historyType.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}