using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IRepairComplaintValidator
{
    Task<List<ValidationResult>> AddNew(RepairComplaint repairComplaint);
    Task<List<ValidationResult>> Update(RepairComplaint repairComplaint);
}

public class RepairComplaintValidator : IRepairComplaintValidator
{
    private readonly IRepairComplaintUnitOfWork _repairComplaintUnitOfWork;

    public RepairComplaintValidator(IRepairComplaintUnitOfWork repairComplaintUnitOfWork)
    {
        _repairComplaintUnitOfWork = repairComplaintUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(RepairComplaint repairComplaint)
    {
        var validationResults = _ValidateBase(repairComplaint);
        if (validationResults.Count == 0)
        {
            var existing = await _repairComplaintUnitOfWork
                .GetRepairComplaints(a => a.Name == repairComplaint.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(RepairComplaint repairComplaint)
    {
        var validationResults = _ValidateBase(repairComplaint);
        if (validationResults.Count == 0)
        {
            var existing = await _repairComplaintUnitOfWork
                .GetRepairComplaints(
                    a => a.Name == repairComplaint.Name && a.Id != repairComplaint.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(RepairComplaint repairComplaint)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(repairComplaint.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));

        return validationResults;
    }
}