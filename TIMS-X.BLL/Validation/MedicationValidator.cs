using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IMedicationValidator
{
    Task<List<ValidationResult>> AddNew(Medication medication);
    Task<List<ValidationResult>> Update(Medication medication);
}

public class MedicationValidator : IMedicationValidator
{
    private readonly IMedicationUnitOfWork _medicationUnitOfWork;

    public MedicationValidator(IMedicationUnitOfWork medicationUnitOfWork)
    {
        _medicationUnitOfWork = medicationUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(Medication medication)
    {
        var validationResults = _ValidateBase(medication);
        if (validationResults.Count == 0)
        {
            var existing = await _medicationUnitOfWork
                .GetMedications(a => a.Name == medication.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(Medication medication)
    {
        var validationResults = _ValidateBase(medication);
        if (validationResults.Count == 0)
        {
            var existing = await _medicationUnitOfWork
                .GetMedications(
                    a => a.Name == medication.Name && a.Id != medication.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(Medication medication)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(medication.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}