using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TIMS_X.Core;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Models;
using TIMS_X.Core.Models.Legacy;
using TIMS_X.Server.Filters;
using TIMS_X.Server.Queries;

namespace TIMS_X.Server.Services;

public class SchedulerService
{
	private readonly ClaimsQuery _claimsQuery;
	private readonly ContextHelper _contextHelper;
	private readonly HistoryQuery _historyQuery;
	private readonly OpportunityTrackingService _opportunityTrackingService;
	private readonly PracticeQuery _practiceQuery;
	private readonly ProviderQuery _providerQuery;
	private readonly SchedulerQuery _schedulerQuery;
	private readonly UserService _userService;

	public SchedulerService(SchedulerQuery schedulerQuery,
		PracticeQuery practiceQuery,
		ProviderQuery providerQuery,
		ClaimsQuery claimsQuery,
		UserService userService,
		OpportunityTrackingService otService,
		HistoryQuery historyQuery,
		ContextHelper contextHelper)
	{
		_schedulerQuery = schedulerQuery;
		_practiceQuery = practiceQuery;
		_providerQuery = providerQuery;
		_userService = userService;
		_claimsQuery = claimsQuery;
		_opportunityTrackingService = otService;
		_historyQuery = historyQuery;
		_contextHelper = contextHelper;
	}

	private List<DateTime> _GetIndividualDays(DateTime fromDate, DateTime toDate)
	{
		var dates = new List<DateTime>();
		var from = fromDate.Date;
		var to = toDate.Date;
		while (from <= to)
		{
			dates.Add(from);
			from = from.AddDays(1);
		}

		return dates;
	}

	private void _MapAppointmentUpdateModelToAppointment(AppointmentUpdateModel updateModel,
		Appointment appointment)
	{
		appointment.Id = updateModel.Id;
		appointment.PatientId = updateModel.PatientId;
		appointment.ProviderId = updateModel.ProviderId;
		appointment.SiteId = updateModel.SiteId;
		appointment.StartsAt = updateModel.StartsAt;
		appointment.EndsAt = updateModel.EndsAt;
		appointment.Notes = updateModel.Notes;
		appointment.AppointmentStatusId = updateModel.AppointmentStatusId;
		appointment.AppointmentTypeId = updateModel.AppointmentTypeId;
		appointment.AuthorizationId = updateModel.AuthorizationId;
		appointment.BillToProviderId = updateModel.BillToProviderId;
		appointment.Custom1 = updateModel.Custom1;
		appointment.Custom2 = updateModel.Custom2;
		appointment.MarketingId = updateModel.MarketingReferenceId ?? 0;
		appointment.NextContactDate = updateModel.NextContact;
		appointment.NotificationStatus = (NotificationStatus)(updateModel.NotificationStatus ?? 0);
		appointment.AddToCancellationList = updateModel.AddToCancellationList;
		appointment.ResourceId = updateModel.ResourceId ?? 0;
		appointment.OtStatus = updateModel.OpportunityStatus;
	}

	private async Task<Dictionary<string, List<string>>> _ValidateAppointmentAsync(Appointment appointment,
		bool ignoreWarnings)
	{
		var businessRules = await _practiceQuery.GetBusinessRulesAsync();
		var result = new Dictionary<string, List<string>>();
		result["errors"] = new List<string>();
		result["warnings"] = new List<string>();


		// Site is required

		if (appointment.SiteId == 0)
			result["errors"].Add("Site is required.");
		else if (!await _practiceQuery.ValidateSiteAsync(appointment.SiteId))
			result["errors"].Add($"Site with ID {appointment.SiteId} is inactive or doesn't exist.");

		// Provider is required

		if (appointment.ProviderId == 0)
		{
			result["errors"].Add("Provider is required.");
		}
		else if (!await _providerQuery.ValidateProviderAsync(appointment.ProviderId))
		{
			result["errors"].Add($"Provider with ID {appointment.ProviderId} is inactive or doesn't exist.");
		}
		else
		{
			var provider = appointment.Provider;
			if (provider == null)
				provider = await _providerQuery.GetProviderAsync(appointment.ProviderId);
			var providerAtSite = await _userService.IsUserAtSiteAsync(provider.UserId, appointment.SiteId,
				appointment.StartsAt, appointment.EndsAt);

			if (!providerAtSite)
				result["warnings"]
					.Add(
						$"Provider {provider.FullName} is not scheduled to be at the appointment site at the appointment time.");
		}


		// Appointment time/duration requirements

		if (appointment.StartsAt >= appointment.EndsAt)
			result["errors"].Add("Appointment start time cannot be greater than end time.");

		if (appointment.StartsAt.Date != appointment.EndsAt.Date) result["errors"].Add("Appointment cannot span days.");

		var temp = DateTime.Now;
		var now = new DateTime(temp.Year, temp.Month, temp.Day, temp.Hour, temp.Minute, 0);

		if (appointment.StartsAt < now) result["warnings"].Add("The appointment is being made for a time in the past.");

		// Appointment type is required based on business rule

		if (!appointment.AppointmentTypeId.HasValue || appointment.AppointmentTypeId == 0)
		{
			if (businessRules.RequireAppointmentType) result["errors"].Add("Appointment Type is required.");
		}
		else if (!await _schedulerQuery.ValidateAppointmentTypeAsync(appointment.AppointmentTypeId.Value))
		{
			result["errors"]
				.Add($"Appointment Type with ID {appointment.AppointmentTypeId} is inactive or doesn't exist.");
		}

		return result;
	}

	public async Task CreateAppointmentAsync(AppointmentCreateModel createModel)
	{
		var appointment = new Appointment(createModel);

		(appointment.OtStatus, _) =
			await _opportunityTrackingService.GetAppointmentOpportunityStatusAsync(appointment.PatientId);
		// Validation needs provider object
		appointment.Provider = await _providerQuery.GetProviderAsync(appointment.ProviderId);
		var results = await _ValidateAppointmentAsync(appointment, false);
		appointment.Provider = null;

		appointment.UpdatedSiteId = appointment.SyncSiteId = appointment.SiteId;
		appointment.UpdatedUserId = _contextHelper.CurrentUser.Id;
		appointment.CreatedUserId = appointment.UpdatedUserId;
		await _schedulerQuery.PutAppointmentAsync(appointment);
		if (appointment.Id > 0)
		{
			createModel.Id = appointment.Id;
			await _historyQuery.CreateHistoryForAppointmentAsync(appointment);
		}
	}

	public async Task DeleteAppointmentAsync(int appointmentId)
	{
		var appointment = await _schedulerQuery.GetAppointmentAsync(appointmentId, false);

		if (appointment != null)
		{
			var posDocuments = await _claimsQuery.GetPosDocumentsForAppointmentAsync(appointmentId);
			if (posDocuments.Any(x => x.DocumentType == PosDocumentType.Invoice))
				throw new ValidationException(
					$"Appointment with ID {appointmentId} has an associated invoice and cannot be deleted.");
			var claimTransactions = await _claimsQuery.GetClaimTransactionsForAppointmentAsync(appointmentId);
			if (claimTransactions.Any())
				throw new ValidationException(
					$"Appointment with ID {appointmentId} has an associated claim and cannot be deleted.");

			await _historyQuery.DeleteHistoryForPendingAppointmentAsync(appointment);
			_schedulerQuery.DeleteAppointment(appointment);
		}
	}

	public async Task<IEnumerable<DateTime>> FindAppointmentOpeningsAsync(SchedulerQueryFilter queryFilter,
		int interval)
	{
		var openings = new List<DateTime>();

		var siteId = queryFilter.SiteIds.FirstOrDefault();
		var providerId = queryFilter.ProviderIds.FirstOrDefault();

		if (queryFilter.From > queryFilter.To || siteId == 0 || providerId == 0) return openings;

		var scheduleItems = (await _schedulerQuery.SearchAppointmentsAsync(queryFilter)).ToList();

		var days = _GetIndividualDays(queryFilter.From, queryFilter.To);

		foreach (var day in days)
		{
			var siteHours = await _practiceQuery.GetSiteHoursAsync(siteId, day.DayOfWeek);

			if (siteHours == null || !siteHours.Item1.HasValue || !siteHours.Item2.HasValue) continue;

			var currentTime = day.Date.Add(siteHours.Item1.Value.TimeOfDay);
			var endTime = day.Date.Add(siteHours.Item2.Value.TimeOfDay);

			if (currentTime < queryFilter.From)
				currentTime = queryFilter.From;

			while (currentTime < endTime)
			{
				var nextTime = currentTime.AddMinutes(interval);
				// Check if appointment start/end times overlap time interval [currentTime, nextTime]
				if (!scheduleItems.Any(x =>
					    (x.FromDate >= currentTime && x.FromDate < nextTime) ||
					    (x.ToDate > currentTime && x.ToDate <= nextTime) ||
					    (x.FromDate <= currentTime && x.ToDate >= nextTime)))
					openings.Add(currentTime);
				currentTime = nextTime;
			}
		}

		return openings;
	}

	public async Task<NotificationResult> GetNotificationResultAsync(MessageTemplateType templateType,
		MessageDeliveryMethod deliveryMethod, int appointmentId)
	{
		return await _schedulerQuery.GetNotificationResultAsync(templateType, deliveryMethod, appointmentId);
	}

	public async Task<List<NotificationResult>> GetNotificationResultsAsync(MessageTemplateType templateType,
		MessageDeliveryMethod[] deliveryMethods, int[] appointmentIds)
	{
		return await _schedulerQuery.GetNotificationResultsAsync(templateType, deliveryMethods, appointmentIds);
	}

	public async Task<Dictionary<string, List<string>>> UpdateAppointmentAsync(AppointmentUpdateModel updateAppointment,
		bool ignoreWarnings)
	{
		Dictionary<string, List<string>> result;

		var appointment = await _schedulerQuery.GetAppointmentAsync(updateAppointment.Id, true);
		if (appointment == null)
		{
			result = new Dictionary<string, List<string>>();
			result["errors"] = new List<string>();
			result["warnings"] = new List<string>();
			result["errors"].Add($"Appointment with ID {updateAppointment.Id} does not exist.");
			return result;
		}

		_MapAppointmentUpdateModelToAppointment(updateAppointment, appointment);


		result = await _ValidateAppointmentAsync(appointment, ignoreWarnings);

		if (result["errors"].Any())
			return result;
		if (!ignoreWarnings && result["warnings"].Any())
			return result;

		if (appointment.BillToProviderId == 0 || appointment.BillToProviderId != appointment.ProviderId)
			// Either bill the provider directly or delegate.
			appointment.BillToProviderId = appointment.Provider.NotBillable
				? appointment.Provider.BillToId
				: appointment.Provider.Id;

		appointment.UpdatedSiteId = appointment.SyncSiteId = appointment.SiteId;
		appointment.UpdatedUserId = _contextHelper.CurrentUser.Id;
		appointment.UpdatedDate = DateTime.Now;

		await _schedulerQuery.PutAppointmentAsync(appointment);

		return result;
	}
}