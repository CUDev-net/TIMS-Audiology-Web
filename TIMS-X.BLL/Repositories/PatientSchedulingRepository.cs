using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tims.Dal.Models;
using TIMS_X.BLL.Services;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;
using TIMS_X.DAL.Dtos;

namespace TIMS_X.BLL.Repositories;

public interface IPatientSchedulingRepository
{
	Task<CreatedPatientAppointmentDto> Create(PatientScheduledAppointmentDto patientScheduledAppointmentDto);
	Task<List<TimeSlotDto>> GetTimeSlots(int providerId, int siteId, DateTime date);
}

public class PatientSchedulingRepository : IPatientSchedulingRepository
{
	private readonly IAppointmentBusinessRuleService _appointmentBusinessRuleService;
	private readonly IAppointmentStatusUnitOfWork _appointmentStatusUnitOfWork;
	private readonly IAppointmentsUnitOfWork _appointmentsUnitOfWork;
	private readonly IAppointmentTypeUnitOfWork _appointmentTypeUnitOfWork;
	private readonly IPatientsUnitOfWork _patientUnitOfWork;
	private readonly IPracticeUnitOfWork _practiceUnitOfWork;
	private readonly IProvidersUnitOfWork _providersUnitOfWork;
	private readonly IScheduleUnitOfWork _scheduleUnitOfWork;
	private readonly ISiteUnitOfWork _siteUnitOfWork;
	private readonly IUserUnitOfWork _userUnitOfWork;

	public PatientSchedulingRepository(IProvidersUnitOfWork providersUnitOfWork,
		IAppointmentsUnitOfWork appointmentsUnitOfWork,
		IAppointmentTypeUnitOfWork appointmentTypeUnitOfWork,
		IAppointmentBusinessRuleService appointmentBusinessRuleService,
		IPatientsUnitOfWork patientUnitOfWork,
		IPracticeUnitOfWork practiceUnitOfWork,
		IAppointmentStatusUnitOfWork appointmentStatusUnitOfWork,
		IScheduleUnitOfWork scheduleUnitOfWork,
		IUserUnitOfWork userUnitOfWork,
		ISiteUnitOfWork siteUnitOfWork)
	{
		_providersUnitOfWork = providersUnitOfWork;
		_appointmentsUnitOfWork = appointmentsUnitOfWork;
		_appointmentTypeUnitOfWork = appointmentTypeUnitOfWork;
		_appointmentBusinessRuleService = appointmentBusinessRuleService;
		_patientUnitOfWork = patientUnitOfWork;
		_practiceUnitOfWork = practiceUnitOfWork;
		_appointmentStatusUnitOfWork = appointmentStatusUnitOfWork;
		_scheduleUnitOfWork = scheduleUnitOfWork;
		_userUnitOfWork = userUnitOfWork;
		_siteUnitOfWork = siteUnitOfWork;
	}

	public async Task<List<TimeSlotDto>> GetTimeSlots(int providerId, int siteId, DateTime date)
	{
		if (providerId > 0)
			return await _GetTimeSlots(providerId, siteId, date);

		var providers = await _providersUnitOfWork.GetProviderSummaries(x => !x.Inactive && x.UseForPatientScheduling)
			.ToListAsync();
		var slots = new List<TimeSlotDto>();

		foreach (var provider in providers)
		{
			var providerSlots = await _GetTimeSlots(provider.Id, siteId, date);
			foreach (var slot in providerSlots)
			{
				var existingSlot = slots.FirstOrDefault(x => x.TimeSlot == slot.TimeSlot);
				if (existingSlot == null)
					slots.AddRange(providerSlots);
			}
		}

		return slots;
	}

	public async Task<CreatedPatientAppointmentDto> Create(
		PatientScheduledAppointmentDto patientScheduledAppointmentDto)
	{
		patientScheduledAppointmentDto.FirstName = patientScheduledAppointmentDto.FirstName.Trim();
		patientScheduledAppointmentDto.LastName = patientScheduledAppointmentDto.LastName.Trim();
		patientScheduledAppointmentDto.MiddleInitial = patientScheduledAppointmentDto.MiddleInitial.Trim();
		patientScheduledAppointmentDto.Phone = patientScheduledAppointmentDto.Phone.Trim();
		patientScheduledAppointmentDto.Email = patientScheduledAppointmentDto.Email.Trim();

		var appointmentType =
			await _appointmentTypeUnitOfWork.GetAppointmentTypes(x => x.Name.ToLower() == "online");
		var businessRules = await _practiceUnitOfWork.GetPracticeBusinessRules();
		var practice = await _practiceUnitOfWork.GetPracticeSummary();
		var status = await _appointmentStatusUnitOfWork.GetAppointmentStatuses(s => s.Name.ToLower() == "new");

		var toCreate = new Appointment
		{
			CreatedUserId = 0,
			UpdatedUserId = 0,
			UpdatedSiteId = patientScheduledAppointmentDto.SiteId,
			ProviderId = patientScheduledAppointmentDto.ProviderId,
			SiteId = patientScheduledAppointmentDto.SiteId,
			AppointmentTypeId = appointmentType.Single().Id,
			AppointmentStatusId = status.Single().Id,
			StartsAt = patientScheduledAppointmentDto.Date,
			EndsAt = patientScheduledAppointmentDto.Date.AddMinutes(60),
			MarketingId = 0
		};
		toCreate.ResourceId ??= 0;
		_appointmentBusinessRuleService.ApplyBillToProviderRule(toCreate).GetAwaiter().GetResult();
		toCreate.PatientId = (int)businessRules.OnlineAppointmentPatientId;
		if (!patientScheduledAppointmentDto.IsNewPatient)
			try
			{
				var patients = await _patientUnitOfWork.FindPatients(new PatientSearchCriteriaDto
				{
					FirstName = patientScheduledAppointmentDto.FirstName,
					LastName = patientScheduledAppointmentDto.LastName,
					DateOfBirth = patientScheduledAppointmentDto.BirthDate.Date,
					PhoneNumber = patientScheduledAppointmentDto.Phone,
					Email = patientScheduledAppointmentDto.Email
				});
				if (patients.Count == 1)
				{
					var p = patients.Single();
					toCreate.PatientId = p.Id;
					toCreate.OtStatus = p.OtStatusId;
					toCreate.OtStatusDescriptionId = p.OtStatusDescriptionId;
				}
			}
			catch (Exception)
			{
			}

		var isNew = patientScheduledAppointmentDto.IsNewPatient ? "New Patient" : "Existing Patient";
		toCreate.Notes =
			$"{isNew}: {patientScheduledAppointmentDto.FirstName} {patientScheduledAppointmentDto.MiddleInitial} {patientScheduledAppointmentDto.LastName}{Environment.NewLine}DOB: {patientScheduledAppointmentDto.BirthDate:d}{Environment.NewLine}Phone: {patientScheduledAppointmentDto.Phone}{Environment.NewLine}Email: {patientScheduledAppointmentDto.Email}";

		if (!string.IsNullOrEmpty(toCreate.Notes)) toCreate.Notes += Environment.NewLine;
		toCreate.Notes += "Reason: " + patientScheduledAppointmentDto.Reason;
		try
		{
			var created = await _appointmentsUnitOfWork.Add(toCreate);
			var site = await _siteUnitOfWork.GetSite(created.SiteId);
			var provider = await _providersUnitOfWork.GetProvider(created.ProviderId);

			var datestring = $"{patientScheduledAppointmentDto.Date:D}";
			if (practice.Locale == "NZ")
			{
				datestring = patientScheduledAppointmentDto.Date.ToString("D", new CultureInfo("en-NZ"));
			}

			var createdDto = new CreatedPatientAppointmentDto
			{
				Appointment = created,
				Message =
					$"You have requested an appointment for {datestring} at {patientScheduledAppointmentDto.Date:t} with {provider.FirstName} {provider.LastName} at the {site.Name} office.",
				EmailMessage = $"An email will be sent to {patientScheduledAppointmentDto.Email}.",
				PendingMessage = $"The appointment is pending until you receive a confirmation from {practice.Name}.",
				PracticeMessage = businessRules.OnlinePatientAppointmentMessage
			};
			
			return createdDto;
		}
		catch (Exception)
		{
			return null;
		}
	}

	private bool _DoAppointmentConflictsExist(List<AppointmentItemSummary> appointments, TimeSpan start, TimeSpan end)
	{
		var conflictExists = false;
		if (appointments.Any())
		{
			var startConflicts = appointments.Where(a =>
				a.start_date.TimeOfDay <= start &&
				a.end_date.TimeOfDay > start);

			var duringConflicts = appointments.Where(a =>
				a.start_date.TimeOfDay >= start &&
				a.end_date.TimeOfDay <= end);

			var endConflicts = appointments.Where(a =>
				a.start_date.TimeOfDay < end &&
				a.end_date.TimeOfDay >= end);
			conflictExists = startConflicts.Any() || duringConflicts.Any() || endConflicts.Any();
		}

		return conflictExists;
	}

	private bool _DoRecurringScheduleConflictsExist(List<ScheduleRecurringItemSummary> schedules, TimeSpan start,
		TimeSpan end)
	{
		var conflictExists = false;
		if (schedules.Any())
		{
			var startConflicts = schedules.Where(a =>
				a.start_date.TimeOfDay <= start &&
				a.end_date.TimeOfDay > start);

			var duringConflicts = schedules.Where(a =>
				a.start_date.TimeOfDay >= start &&
				a.end_date.TimeOfDay <= end);

			var endConflicts = schedules.Where(a =>
				a.start_date.TimeOfDay < end &&
				a.end_date.TimeOfDay >= end);
			conflictExists = startConflicts.Any() || duringConflicts.Any() || endConflicts.Any();
		}

		return conflictExists;
	}

	private bool _DoScheduleConflictsExist(List<ScheduleItemSummary> schedules, TimeSpan start, TimeSpan end)
	{
		var conflictExists = false;
		if (schedules.Any())
		{
			var startConflicts = schedules.Where(a =>
				a.start_date.TimeOfDay <= start &&
				a.end_date.TimeOfDay > start);

			var duringConflicts = schedules.Where(a =>
				a.start_date.TimeOfDay >= start &&
				a.end_date.TimeOfDay <= end);

			var endConflicts = schedules.Where(a =>
				a.start_date.TimeOfDay < end &&
				a.end_date.TimeOfDay >= end);
			conflictExists = startConflicts.Any() || duringConflicts.Any() || endConflicts.Any();
		}

		return conflictExists;
	}

	private static List<DateTime> _Get30MinuteIntervals(DateTime start, DateTime end)
	{
		var intervals = new List<DateTime>();

		for (var time = start; time < end; time = time.AddMinutes(30)) intervals.Add(time);

		return intervals;
	}

	private async Task<List<TimeSlotDto>> _GetTimeSlots(int providerId, int siteId, DateTime date)
	{
		var timsDayOfWeek = ScheduleOpeningsRepository.ConvertMStoTIMSDayOfWeek(date.DayOfWeek);
		var provider = await _providersUnitOfWork.GetProviderSummary(providerId);
		var user = await _userUnitOfWork.GetUser(provider.UserId);
		// Hack to get the summaries to filter correctly
		user.ScheduleProviderFilter = providerId.ToString();
		var hours = await _providersUnitOfWork.GetSingleProviderHours(providerId, siteId, timsDayOfWeek);

		var recurringSchedules = await _scheduleUnitOfWork.GetRecurringScheduleSummaries(date, date, user);
		var schedules = await _scheduleUnitOfWork.GetScheduleSummaries(date.Date, date.Date.AddDays(1), user)
			.ToListAsync();
		var appointments = await _appointmentsUnitOfWork.GetAppointmentSummaries(
			date.Date, date.Date.AddDays(1), user).ToListAsync();
		var isToday = date.Date == DateTime.Today.Date;
		var slots = new List<TimeSlotDto>();
		foreach (var hoursOfOperationModel in hours)
			if (hoursOfOperationModel.StartTime.HasValue && hoursOfOperationModel.EndTime.HasValue)
			{
				var timeSlots = _Get30MinuteIntervals(hoursOfOperationModel.StartTime.Value,
					hoursOfOperationModel.EndTime.Value);

				foreach (var timeSlot in timeSlots)
				{
					if (isToday && timeSlot.TimeOfDay < date.TimeOfDay)
						continue;

					var start = timeSlot.TimeOfDay;
					var end = timeSlot.TimeOfDay.Add(new TimeSpan(1, 0, 0));
					if (end > hoursOfOperationModel.EndTime.Value.TimeOfDay)
						continue;

					var appointmentConflictsExist = _DoAppointmentConflictsExist(appointments, start, end);
					var scheduleConflictsExist = _DoScheduleConflictsExist(schedules, start, end);
					var recurringScheduleConflictsExist =
						_DoRecurringScheduleConflictsExist(recurringSchedules, start, end);
					if (!appointmentConflictsExist && !scheduleConflictsExist && !recurringScheduleConflictsExist)
						slots.Add(new TimeSlotDto { TimeSlot = timeSlot, ProviderId = providerId });
				}
			}

		return slots;
	}
}