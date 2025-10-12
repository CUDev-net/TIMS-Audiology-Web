using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IPreviousHistoryValidator
{
    Task<List<ValidationResult>> AddNew(PreviousHistory previousHistory);
    Task<List<ValidationResult>> Update(PreviousHistory previousHistory);
}

public class PreviousHistoryValidator : IPreviousHistoryValidator
{
    private readonly IMedicalConditionUnitOfWork _medicalConditionUnitOfWork;

    public PreviousHistoryValidator(IMedicalConditionUnitOfWork medicalConditionUnitOfWork)
    {
        _medicalConditionUnitOfWork = medicalConditionUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(PreviousHistory previousHistory)
    {
        var validationResults = _ValidateBase(previousHistory);
        if (validationResults.Count == 0)
        {
            var existing = await _medicalConditionUnitOfWork.GetMedicalCondition(previousHistory.ConditionId);
            if (existing != null)
                validationResults.Add(new ValidationResult("Patient medical condition must exist", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(PreviousHistory previousHistory)
    {
        var validationResults = _ValidateBase(previousHistory);
        if (validationResults.Count == 0)
        {
            var existing = await _medicalConditionUnitOfWork.GetMedicalCondition(previousHistory.ConditionId);
            if (existing != null)
                validationResults.Add(new ValidationResult("Patient medical condition must exist", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(PreviousHistory previousHistory)
    {
        var validationResults = new List<ValidationResult>();
        if (!previousHistory.Protected)
            validationResults.Add(new ValidationResult("Previous History must exist", Severity.Error));

        return validationResults;
    }
}