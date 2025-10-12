using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IMedicalConditionValidator
{
    Task<List<ValidationResult>> AddNew(MedicalCondition medicalCondition);
    Task<List<ValidationResult>> Update(MedicalCondition medicalCondition);
}

public class MedicalConditionValidator : IMedicalConditionValidator
{
    private readonly IMedicalConditionUnitOfWork _medicalConditionUnitOfWork;

    public MedicalConditionValidator(IMedicalConditionUnitOfWork medicalConditionUnitOfWork)
    {
        _medicalConditionUnitOfWork = medicalConditionUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(MedicalCondition medicalCondition)
    {
        var validationResults = _ValidateBase(medicalCondition);
        if (validationResults.Count == 0)
        {
            var existing = await _medicalConditionUnitOfWork
                .GetMedicalConditions(a => a.Name == medicalCondition.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(MedicalCondition medicalCondition)
    {
        var validationResults = _ValidateBase(medicalCondition);
        if (validationResults.Count == 0)
        {
            var existing = await _medicalConditionUnitOfWork
                .GetMedicalConditions(
                    a => a.Name == medicalCondition.Name && a.Id != medicalCondition.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(MedicalCondition medicalCondition)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(medicalCondition.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));

        return validationResults;
    }
}