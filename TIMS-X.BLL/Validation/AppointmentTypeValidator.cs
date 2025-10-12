using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IAppointmentTypeValidator
{
    Task<List<ValidationResult>> AddNew(AppointmentType appointmentType);
    Task<List<ValidationResult>> Update(AppointmentType appointmentType);
}

public class AppointmentTypeValidator : IAppointmentTypeValidator
{
    private readonly IAppointmentTypeUnitOfWork _appointmentTypeUnitOfWork;
    private readonly IHistoryTypeUnitOfWork _historyTypeUnitOfWork;

    public AppointmentTypeValidator(IAppointmentTypeUnitOfWork appointmentTypeUnitOfWork,
        IHistoryTypeUnitOfWork historyTypeUnitOfWork)
    {
        _appointmentTypeUnitOfWork = appointmentTypeUnitOfWork;
        _historyTypeUnitOfWork = historyTypeUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(AppointmentType appointmentType)
    {
        var validationResults = await _ValidateBase(appointmentType);
        if (validationResults.Count == 0)
        {
            var existing = await _appointmentTypeUnitOfWork
                .GetAppointmentTypes(a => a.Name == appointmentType.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(AppointmentType appointmentType)
    {
        var validationResults = await _ValidateBase(appointmentType);
        if (validationResults.Count == 0)
        {
            var existing = await _appointmentTypeUnitOfWork
                .GetAppointmentTypes(
                    a => a.Name == appointmentType.Name && a.Id != appointmentType.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private async Task<List<ValidationResult>> _ValidateBase(AppointmentType appointmentType)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(appointmentType.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        if (appointmentType.Duration <= 0)
            validationResults.Add(new ValidationResult("Duration must be longer than 0 seconds", Severity.Error));
        if (appointmentType.HistoryTypeId != null && appointmentType.HistoryTypeId > 0)
        {
            var historyType = await _historyTypeUnitOfWork.GetHistoryType(appointmentType.HistoryTypeId.Value);
            if (historyType == null)
                validationResults.Add(new ValidationResult("History type does not exist", Severity.Error));
        }

        return validationResults;
    }
}