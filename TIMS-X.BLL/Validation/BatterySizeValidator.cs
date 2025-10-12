using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IBatterySizeValidator
{
    Task<List<ValidationResult>> AddNew(BatterySize batterySize);
    Task<List<ValidationResult>> Update(BatterySize batterySize);
}

public class BatterySizeValidator : IBatterySizeValidator
{
    private readonly IBatterySizeUnitOfWork _batterySizeUnitOfWork;

    public BatterySizeValidator(IBatterySizeUnitOfWork batterySizeUnitOfWork)
    {
        _batterySizeUnitOfWork = batterySizeUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(BatterySize batterySize)
    {
        var validationResults = _ValidateBase(batterySize);
        if (validationResults.Count == 0)
        {
            var existing = await _batterySizeUnitOfWork
                .GetBatterySizes(a => a.Name == batterySize.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(BatterySize batterySize)
    {
        var validationResults = _ValidateBase(batterySize);
        if (validationResults.Count == 0)
        {
            var existing = await _batterySizeUnitOfWork
                .GetBatterySizes(
                    a => a.Name == batterySize.Name && a.Id != batterySize.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(BatterySize batterySize)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(batterySize.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}