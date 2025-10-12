using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IHaReturnReasonValidator
{
    Task<List<ValidationResult>> AddNew(HaReturnReason haReturnReason);
    Task<List<ValidationResult>> Update(HaReturnReason haReturnReason);
}

public class HaReturnReasonValidator : IHaReturnReasonValidator
{
    private readonly IHaReturnReasonUnitOfWork _haReturnReasonUnitOfWork;

    public HaReturnReasonValidator(IHaReturnReasonUnitOfWork haReturnReasonUnitOfWork)
    {
        _haReturnReasonUnitOfWork = haReturnReasonUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(HaReturnReason haReturnReason)
    {
        var validationResults = _ValidateBase(haReturnReason);
        if (validationResults.Count == 0)
        {
            var existing = await _haReturnReasonUnitOfWork
                .GetReturnReasons(a => a.Name == haReturnReason.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(HaReturnReason haReturnReason)
    {
        var validationResults = _ValidateBase(haReturnReason);
        if (validationResults.Count == 0)
        {
            var existing = await _haReturnReasonUnitOfWork
                .GetReturnReasons(
                    a => a.Name == haReturnReason.Name && a.Id != haReturnReason.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(HaReturnReason haReturnReason)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(haReturnReason.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));

        return validationResults;
    }
}