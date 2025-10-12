using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Extensions;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Validation;

public interface IAppointmentValidator
{
    Task<List<ValidationResult>> AddNew(Appointment appointment);
    List<ValidationResult> Delete(int id);
    Task<List<ValidationResult>> Update(Appointment appointment);
}

public class AppointmentValidator : BaseValidator, IAppointmentValidator
{
    private readonly IAppointmentsUnitOfWork _appointmentsUnitOfWork;
    private readonly IAppointmentTypeUnitOfWork _appointmentTypeUnitOfWork;
    private readonly IPracticeUnitOfWork _practiceUnitOfWork;
    private readonly IProvidersUnitOfWork _providersUnitOfWork;
    private readonly IScheduleBlockUnitOfWork _scheduleBlockUnitOfWork;
    private readonly ISiteUnitOfWork _siteRepository;
    private readonly ITimsUserSiteUnitOfWork _timsUserSiteUnitOfWork;
    private readonly IUserUnitOfWork _timsUserUnitOfWork;


    public AppointmentValidator(IAppointmentsUnitOfWork appointmentsUnitOfWork,
        ISiteUnitOfWork siteRepository,
        IProvidersUnitOfWork providersUnitOfWork,
        IUserUnitOfWork timsUserUnitOfWork,
        ITimsUserSiteUnitOfWork timsUserSiteUnitOfWork,
        IPracticeUnitOfWork practiceUnitOfWork,
        IAppointmentTypeUnitOfWork appointmentTypeUnitOfWork,
        IScheduleBlockUnitOfWork scheduleBlockUnitOfWork)
    {
        _appointmentsUnitOfWork = appointmentsUnitOfWork;
        _siteRepository = siteRepository;
        _providersUnitOfWork = providersUnitOfWork;
        _timsUserUnitOfWork = timsUserUnitOfWork;
        _timsUserSiteUnitOfWork = timsUserSiteUnitOfWork;
        _practiceUnitOfWork = practiceUnitOfWork;
        _appointmentTypeUnitOfWork = appointmentTypeUnitOfWork;
        _scheduleBlockUnitOfWork = scheduleBlockUnitOfWork;
    }

    public async Task<List<ValidationResult>> AddNew(Appointment appointment)
    {
        var validationResults = _ValidateBase(appointment);

        var duplicates = _appointmentsUnitOfWork.GetPatientAppointments(appointment.PatientId, appointment.Id,
            appointment.StartsAt,
            appointment.EndsAt);
        if (duplicates.Any())
            validationResults.Add(new ValidationResult("Patient has appointments at this time", Severity.Warning));

        var providerHoursValidation = await ValidateProviderHours(appointment.ProviderId, appointment.SiteId,
            appointment.StartsAt, appointment.EndsAt, _providersUnitOfWork, _timsUserUnitOfWork,
            _timsUserSiteUnitOfWork);
        validationResults.AddRange(providerHoursValidation);

        if (appointment.RecurringInterval != null)
        {
            if (appointment.RecurringInterval.EndType == 3 && appointment.RecurringInterval.EndDate <= appointment.RecurringInterval.StartDate)
            {
                validationResults.Add(new ValidationResult("Recurring end date must be after appointment date", Severity.Error));
            }
        }

        await _ValidateBlockScheduling(appointment, validationResults);

        return SortResults(validationResults);
    }

    public async Task<List<ValidationResult>> Update(Appointment appointment)
    {
        var validationResults = _ValidateBase(appointment);
        await _ValidateBlockScheduling(appointment, validationResults);

        return SortResults(validationResults);
    }

    public List<ValidationResult> Delete(int id)
    {
        var errors = new List<ValidationResult>();
        if (_appointmentsUnitOfWork.HasClaimData(id))
            errors.Add(new ValidationResult("This appointment has an associated claim and cannot be deleted.",
                Severity.Error));
        if (_appointmentsUnitOfWork.HasPosDocument(id))
            errors.Add(new ValidationResult("This appointment has an associated invoice and cannot be deleted.",
                Severity.Error));
        return errors;
    }

    private List<ValidationResult> _ValidateBase(Appointment appointment)
    {
        var validationResults = new List<ValidationResult>();
        if (appointment.AppointmentTypeId == 0)
            validationResults.Add(new ValidationResult("Appointment Type is required", Severity.Error));
        if (appointment.AppointmentStatusId == 0)
            validationResults.Add(new ValidationResult("Appointment Status is required", Severity.Error));
        if (appointment.PatientId == 0)
            validationResults.Add(new ValidationResult("Patient is required", Severity.Error));
        if (appointment.EndsAt <= appointment.StartsAt)
            validationResults.Add(new ValidationResult("End must be after Start", Severity.Error));
        if (appointment.ProviderId == 0)
            validationResults.Add(new ValidationResult("Provider is required", Severity.Error));
        if (appointment.SiteId == 0) validationResults.Add(new ValidationResult("Site is required", Severity.Error));
        if (appointment.StartsAt <= DateTime.Now)
            validationResults.Add(new ValidationResult("Appointment is being made in the past", Severity.Warning));

        return validationResults;
    }

    private async Task _ValidateBlockScheduling(Appointment appointment, List<ValidationResult> validationResults)
    {
        var practice = await _practiceUnitOfWork.GetPracticeSummary();
        if (practice.UseBlockScheduling)
        {
            var appointmentDayOfWeek = (int)DaysOfWeek.FromDayOfWeek(appointment.StartsAt.DayOfWeek);

            var appointmentTypes = await _appointmentTypeUnitOfWork.GetAppointmentTypes(
                x => x.Id == appointment.AppointmentTypeId,
                null,
                x => x.Include(a => a.ScheduleBlock)
                    .ThenInclude(a => a.ProviderBlockSchedules)
                    .ThenInclude(a => a.ScheduleTimeSlot)
            );
            var appointmentType = appointmentTypes.Single();
            var blockLookupData = await _scheduleBlockUnitOfWork.GetScheduleBlocks( x => x.Inactive == false,
                x => x.Include(a => a.ProviderBlockSchedules)
                    .ThenInclude(a => a.ScheduleTimeSlot));

            if (appointmentType.ScheduleBlock != null && appointmentType.ScheduleBlock.Inactive == false)
            {
                // Warn if the appointment type is in a schedule block and the appointment time is not in the block.
                var blockSchedule = blockLookupData.FirstOrDefault(
                    x => x.Id == appointmentType.ScheduleBlock.Id);
                if (blockSchedule != null)
                {
                    var blocks = blockSchedule.ProviderBlockSchedules.Where(x =>
                        x.ScheduleTimeSlot.DayOfWeek == appointmentDayOfWeek &&
                        x.ScheduleTimeSlot.StartTime <= x.ScheduleTimeSlot.StartTime.SetTime(appointment.StartsAt) &&
                        x.ScheduleTimeSlot.EndTime >= x.ScheduleTimeSlot.EndTime.SetTime(appointment.EndsAt)).ToList();
                    if (!blocks.Any())
                    {
                        validationResults.Add(new ValidationResult(
                            "The appointment time is not in the schedule block for the appointment type.",
                            Severity.Warning));
                    }
                    else
                    {
                        // The time is correct, check the provider and warn if it doesn't match.
                        var blocksWithProvider = from b in blocks
                            where b.ProviderId == appointment.ProviderId
                            select b;
                        if (!blocksWithProvider.Any())
                            validationResults.Add(new ValidationResult(
                                "The provider is not in the schedule block for the appointment type.",
                                Severity.Warning));
                    }
                }
            }
            else
            {
                // Warn if the appointment is in a schedule block for a different appointment type.
                var blocks = blockLookupData.Where(
                    s => s.ProviderBlockSchedules.Any(
                        x => x.ScheduleTimeSlot.DayOfWeek == appointmentDayOfWeek &&
                             x.ScheduleTimeSlot.StartTime <=
                             x.ScheduleTimeSlot.StartTime.SetTime(appointment.StartsAt) &&
                             x.ScheduleTimeSlot.EndTime >= x.ScheduleTimeSlot.EndTime.SetTime(appointment.EndsAt))
                ).Select(x => x.ProviderBlockSchedules).ToList();
                if (blocks.Any())
                    validationResults.Add(new ValidationResult(
                        "The appointment is in a schedule block for another appointment type.",
                        Severity.Warning));
            }
        }
    }
}