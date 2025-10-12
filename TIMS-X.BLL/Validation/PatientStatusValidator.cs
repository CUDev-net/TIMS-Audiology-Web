using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IPatientStatusValidator
{
    Task<List<ValidationResult>> AddNew(PatientStatus patientStatus);
    Task<List<ValidationResult>> Update(PatientStatus patientStatus);
}

public class PatientStatusValidator : IPatientStatusValidator
{
    private readonly IPatientStatusUnitOfWork _patientStatusUnitOfWork;

    public PatientStatusValidator(IPatientStatusUnitOfWork patientStatusUnitOfWork)
    {
        _patientStatusUnitOfWork = patientStatusUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(PatientStatus patientStatus)
    {
        var validationResults = _ValidateBase(patientStatus);
        if (validationResults.Count == 0)
        {
            var existing = await _patientStatusUnitOfWork
                .GetPatientStatuses(a => a.Name == patientStatus.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(PatientStatus patientStatus)
    {
        var validationResults = _ValidateBase(patientStatus);
        if (validationResults.Count == 0)
        {
            var existing = await _patientStatusUnitOfWork
                .GetPatientStatuses(
                    a => a.Name == patientStatus.Name && a.Id != patientStatus.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(PatientStatus patientStatus)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(patientStatus.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}