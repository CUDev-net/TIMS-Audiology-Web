using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tims.Dal.Models;
using TIMS_X.BLL.Services;
using TIMS_X.BLL.Utilities;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.DAL.DAL.UoWs;
using TIMS_X.DAL.Dtos;

namespace TIMS_X.BLL.Repositories;

public interface IAppointmentRepository
{
    Task<ICollection<AppointmentDto>> Add(Appointment appointment);

    void Delete(int id);

    Task<List<int>> EndSeries(int id);
    Task<Appointment> Get(int id);
    Task<List<AppointmentItemSummary>> GetAppointmentSummaries(DateTime startDate, DateTime endDate, int userId);
    Task<AppointmentDto> Update(Appointment appointment);
}

public class AppointmentRepository : IAppointmentRepository
{
    private readonly IAppointmentBusinessRuleService _appointmentBusinessRuleService;
    private readonly IAppointmentsUnitOfWork _appointmentsUnitOfWork;
    private readonly IAppointmentTypeUnitOfWork _appointmentTypeOfWork;
    private readonly IHistoryUnitOfWork _historyUnitOfWork;
    private readonly IPatientsUnitOfWork _patientsUnitOfWork;
    private readonly IPracticeUnitOfWork _practiceUnitOfWork;
    private readonly IProvidersUnitOfWork _providersUnitOfWork;
    private readonly ISiteUnitOfWork _siteUnitOfWork;
    private readonly IUserUnitOfWork _userUnitOfWork;

    public AppointmentRepository(IAppointmentsUnitOfWork appointmentsUnitOfWork,
        IProvidersUnitOfWork providersUnitOfWork,
        ISiteUnitOfWork siteUnitOfWork,
        IAppointmentTypeUnitOfWork appointmentTypeOfWork,
        IPracticeUnitOfWork practiceUnitOfWork,
        IPatientsUnitOfWork patientsUnitOfWork,
        IHistoryUnitOfWork historyUnitOfWork,
        IUserUnitOfWork userUnitOfWork,
        IAppointmentBusinessRuleService appointmentBusinessRuleService)
    {
        _appointmentsUnitOfWork = appointmentsUnitOfWork;
        _providersUnitOfWork = providersUnitOfWork;
        _siteUnitOfWork = siteUnitOfWork;
        _appointmentTypeOfWork = appointmentTypeOfWork;
        _practiceUnitOfWork = practiceUnitOfWork;
        _patientsUnitOfWork = patientsUnitOfWork;
        _historyUnitOfWork = historyUnitOfWork;
        _userUnitOfWork = userUnitOfWork;
        _appointmentBusinessRuleService = appointmentBusinessRuleService;
    }

    public async Task<ICollection<AppointmentDto>> Add(Appointment appointment)
    {
        appointment.ResourceId ??= 0;
        var appointments = new List<AppointmentDto>();

        if (!appointment.MarketingId.HasValue)
	        appointment.MarketingId = 0;

        _appointmentBusinessRuleService.ApplyBillToProviderRule(appointment).GetAwaiter().GetResult();
        var newAppointment = await _appointmentsUnitOfWork.Add(appointment);
        var practice = await _practiceUnitOfWork.GetPracticeSummary();
        var patient = await _patientsUnitOfWork.GetPatient(newAppointment.PatientId);

        newAppointment.OtStatus = (OpportunityStatusEnum)patient.OtStatusId;
        newAppointment.OtStatusDescriptionId = patient.OtStatusDescriptionId;
        
		if (appointment.RecurringInterval == null)
        {
            var added = new AppointmentDto(newAppointment);
            await _SetMetaData(added);

            CreateMetaData(added).GetAwaiter().GetResult();
            appointments.Add(added);
        }
        else
        {
            var occurrences = appointment.RecurringInterval.GetAllOccurrences();
            foreach (var recurringIntervalOccurrence in occurrences)
            {
                var instance = new Appointment
                {
                    PatientId = appointment.PatientId,
                    ProviderId = appointment.ProviderId,
                    SiteId = appointment.SiteId,
                    ResourceId = appointment.ResourceId,
                    MarketingId = appointment.MarketingId,
                    ReferringPhysicianId = appointment.ReferringPhysicianId,
                    BillToProviderId = appointment.BillToProviderId,
                    AppointmentStatusId = appointment.AppointmentStatusId,
                    AppointmentTypeId = appointment.AppointmentTypeId,
                    Notes = appointment.Notes,
                    SyncSiteId = appointment.SyncSiteId,
                    RecurringParentId = appointment.Id,

                    StartsAt = recurringIntervalOccurrence.StartsAt,
                    EndsAt = recurringIntervalOccurrence.EndsAt,
                    RecurringItemNumber = recurringIntervalOccurrence.OccurrenceNumber,
                    OtStatus = appointment.OtStatus,
                    OtStatusDescriptionId = appointment.OtStatusDescriptionId
                };
                _appointmentBusinessRuleService.ApplyBillToProviderRule(instance).GetAwaiter().GetResult();
                var dto = new AppointmentDto(await _appointmentsUnitOfWork.Add(instance));
                await _SetMetaData(dto);

                CreateMetaData(dto).GetAwaiter().GetResult();
                appointments.Add(dto);
            }
        }

        return appointments;

        async Task CreateMetaData(AppointmentDto added)
        {
            added.PatientName = patient.FirstLast;
            if (practice.LinkAppointmentHistory)
            {
                var history = new History
                {
                    AppointmentId = newAppointment.Id,
                    PatientId = newAppointment.PatientId,
                    ProviderId = newAppointment.ProviderId,
                    SyncSiteId = newAppointment.SyncSiteId,
                    ReferralSourceId = patient.ReferringPhysicianId ?? 0,
                    AvailableDate = newAppointment.StartsAt,
                    HistoryDate = newAppointment.StartsAt
                };
                if (newAppointment.AppointmentTypeId.HasValue && newAppointment.AppointmentTypeId.Value > 0)
                {
                    var appointmentType =
                        await _appointmentTypeOfWork.GetAppointmentType(newAppointment.AppointmentTypeId.Value);
                    if (appointmentType is { HistoryTypeId: > 0 })
                        history.HistoryTypeId = appointmentType.HistoryTypeId.Value;
                }

                await _historyUnitOfWork.Add(history);
            }
        }
    }

    public async Task<List<int>> EndSeries(int id)
    {
        if (_appointmentsUnitOfWork.HasClaimData(id) || _appointmentsUnitOfWork.HasPosDocument(id))
            throw new Exception($"Appointment with id {id} has claim or pos data and cannot be deleted");

        var deletedIds = new List<int>();
        var appointment = await _appointmentsUnitOfWork.GetAppointment(id);
        if (appointment == null || !appointment.RecurringParentId.HasValue) return new List<int>();
        var children =
            _appointmentsUnitOfWork.GetAppointments(x =>
                x.RecurringParentId == appointment.RecurringParentId && x.Id > id);

        Delete(id);
        deletedIds.Add(id);
        foreach (var child in children)
        {
            try
            {
                Delete(child.Id);
                deletedIds.Add(child.Id);
            }
            catch (Exception)
            {
                // Probably due to claim data or pos document
                // ignored
            }
        }

        return deletedIds;
    }

    public void Delete(int id)
    {
        if (_appointmentsUnitOfWork.HasClaimData(id) || _appointmentsUnitOfWork.HasPosDocument(id))
            throw new Exception($"Appointment with id {id} has claim or pos data and cannot be deleted");

        _historyUnitOfWork.DeleteByAppointmentId(id);
        _appointmentsUnitOfWork.Delete(id);
    }

    public async Task<Appointment> Get(int id)
    {
        var appointment = await _appointmentsUnitOfWork.GetAppointment(id, x => x.Include(a => a.RecurringInterval).Include(a => a.OtStatusDescription));
        if (appointment == null) return null;
        if (appointment.RecurringParentId.HasValue)
        {
            var parent = await _appointmentsUnitOfWork.GetAppointment(appointment.RecurringParentId.Value,
                x => x.Include(a => a.RecurringInterval));
            appointment.RecurringInterval = parent.RecurringInterval;
        }

        if (appointment.OtStatusDescription != null)
        {
            appointment.OpportunityDescription = appointment.OtStatusDescription.Description;
        }

        if (appointment.UpdatedUserId.HasValue)
        {
            var user = await _userUnitOfWork.GetUser(appointment.UpdatedUserId.Value);
            if (user != null) appointment.UpdatedByUserName = user.Name;
        }

        return appointment;
    }

    public async Task<AppointmentDto> Update(Appointment appointment)
    {
	    if (!appointment.MarketingId.HasValue)
		    appointment.MarketingId = 0;

		appointment.ResourceId ??= 0;
		var updated = new AppointmentDto(await _appointmentsUnitOfWork.Update(appointment));
        await _SetMetaData(updated);
        return updated;
    }

    public async Task<List<AppointmentItemSummary>> GetAppointmentSummaries(DateTime startDate, DateTime endDate,
        int userId)
    {
        var user = await _userUnitOfWork.GetUser(userId);
        var items = await _appointmentsUnitOfWork.GetAppointmentSummaries(startDate, endDate, user).ToListAsync();

        foreach (var item in items)
        {
            item.appointment_type_web_color =
                ColorHelper.GetHexColor(item.appointment_type_color_value);
            item.site_web_color =
                ColorHelper.GetHexColor(item.site_color_value);
            item.provider_web_color =
                ColorHelper.GetHexColor(item.provider_color_value);
        }

        return items;
    }

    private async Task _SetMetaData(AppointmentDto appointment)
    {
        if (appointment.Appointment.AppointmentTypeId != null)
        {
            var appointmentType =
                await _appointmentTypeOfWork.GetAppointmentType(appointment.Appointment.AppointmentTypeId.Value);
            appointment.TypeName = appointmentType.Name;
            appointment.Appointment_Type_Color =
                ColorHelper.GetHexColor(appointmentType.Color);
        }

        var site = await _siteUnitOfWork.GetSite(appointment.Appointment.SiteId);
        appointment.SiteName = site.Name;
        appointment.Site_Color =
            ColorHelper.GetHexColor(site.Color);

        var provider = await _providersUnitOfWork.GetProvider(appointment.Appointment.ProviderId);
        appointment.ProviderName = $"{provider.FirstName} {provider.LastName}";
        appointment.Provider_Color =
            ColorHelper.GetHexColor(provider.Color);
    }
}