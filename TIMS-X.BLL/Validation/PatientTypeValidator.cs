using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IPatientTypeValidator
{
    Task<List<ValidationResult>> AddNew(PatientType patientType);
    Task<List<ValidationResult>> Update(PatientType patientType);
}

public class PatientTypeValidator : IPatientTypeValidator
{
    private readonly IPatientTypeUnitOfWork _patientTypeUnitOfWork;

    public PatientTypeValidator(IPatientTypeUnitOfWork patientTypeUnitOfWork)
    {
        _patientTypeUnitOfWork = patientTypeUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(PatientType patientType)
    {
        var validationResults = _ValidateBase(patientType);
        if (validationResults.Count == 0)
        {
            var existing = await _patientTypeUnitOfWork
                .GetPatientTypes(a => a.Name == patientType.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(PatientType patientType)
    {
        var validationResults = _ValidateBase(patientType);
        if (validationResults.Count == 0)
        {
            var existing = await _patientTypeUnitOfWork
                .GetPatientTypes(
                    a => a.Name == patientType.Name && a.Id != patientType.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(PatientType patientType)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(patientType.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}