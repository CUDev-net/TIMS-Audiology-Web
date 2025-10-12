using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IAppointmentStatusValidator
{
    Task<List<ValidationResult>> AddNew(AppointmentStatus appointment);
    Task<List<ValidationResult>> Update(AppointmentStatus appointment);
}

public class AppointmentStatusValidator : IAppointmentStatusValidator
{
    private readonly IAppointmentStatusUnitOfWork _appointmentStatusUnitOfWork;

    public AppointmentStatusValidator(IAppointmentStatusUnitOfWork appointmentStatusUnitOfWork)
    {
        _appointmentStatusUnitOfWork = appointmentStatusUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(AppointmentStatus appointmentStatus)
    {
        var validationResults = _ValidateBase(appointmentStatus);
        if (validationResults.Count == 0)
        {
            var existing = await _appointmentStatusUnitOfWork
                .GetAppointmentStatuses(a => a.Name == appointmentStatus.Name);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    public async Task<List<ValidationResult>> Update(AppointmentStatus appointmentStatus)
    {
        var validationResults = _ValidateBase(appointmentStatus);
        if (validationResults.Count == 0)
        {
            var existing = await _appointmentStatusUnitOfWork
                .GetAppointmentStatuses(
                    a => a.Name == appointmentStatus.Name && a.Id != appointmentStatus.Id);
            if (existing.Any())
                validationResults.Add(new ValidationResult("Name must be unique", Severity.Error));
        }

        return validationResults;
    }

    private List<ValidationResult> _ValidateBase(AppointmentStatus schedule)
    {
        var validationResults = new List<ValidationResult>();
        if (string.IsNullOrEmpty(schedule.Name))
            validationResults.Add(new ValidationResult("Name is required", Severity.Error));
        return validationResults;
    }
}