using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IHaRepairComplaintValidator
{
    Task<List<ValidationResult>> AddNew(HaRepairComplaint haRepairComplaint);
    Task<List<ValidationResult>> Update(HaRepairComplaint haRepairComplaint);
}

public class HaRepairComplaintValidator : IHaRepairComplaintValidator
{
    private readonly IHaRepairComplaintUnitOfWork _haRepairComplaintUnitOfWork;

    public HaRepairComplaintValidator(IHaRepairComplaintUnitOfWork haRepairComplaintUnitOfWork)
    {
        _haRepairComplaintUnitOfWork = haRepairComplaintUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(HaRepairComplaint haRepairComplaint)
    {
        var validationResults = _ValidateBase(haRepairComplaint);
        if (validationResults.Count == 0)
        {
            var existing = await _haRepairComplaintUnitOfWork
                .GetHaRepairComplaints(a => a.Name == haRepairComplaint.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(HaRepairComplaint haRepairComplaint)
    {
        var validationResults = _ValidateBase(haRepairComplaint);
        if (validationResults.Count == 0)
        {
            var existing = await _haRepairComplaintUnitOfWork
                .GetHaRepairComplaints(
                    a => a.Name == haRepairComplaint.Name && a.Id != haRepairComplaint.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(HaRepairComplaint repairComplaint)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(repairComplaint.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));

        return validationResults;
    }
}