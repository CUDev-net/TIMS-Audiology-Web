using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Extensions;
using TIMS_X.Core.Models.Legacy;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Filters;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;

namespace TIMS_X.Server.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
[ApiExplorerSettings(IgnoreApi = true)]
public class SchedulerController : ControllerBase
{
	private readonly IPatientMessagingService _patientMessagingService;
	private readonly PracticeQuery _practiceQuery;
	private readonly SchedulerQuery _schedulerQuery;
	private readonly SchedulerService _schedulerService;

	public SchedulerController(SchedulerQuery schedulerQuery, PracticeQuery practiceQuery,
		SchedulerService schedulerService,
		IPatientMessagingService patientMessagingService)
	{
		_schedulerQuery = schedulerQuery;
		_practiceQuery = practiceQuery;
		_schedulerService = schedulerService;
		_patientMessagingService = patientMessagingService;
	}

	[HttpPost("CreateAppointment")]
	public async Task<ActionResult> CreateAppointmentAsync(AppointmentCreateModel appointment)
	{
		await _schedulerService.CreateAppointmentAsync(appointment);
		return Ok(appointment.Id);
	}

	[HttpDelete("DeleteAppointment")]
	public async Task<ActionResult> DeleteAppointmentAsync(int appointmentId)
	{
		await _schedulerService.DeleteAppointmentAsync(appointmentId);
		return Ok();
	}

	[HttpGet("AppointmentStatus")]
	public async Task<ActionResult> GetAppointmentStatusAsync(string name)
	{
		var status = await _schedulerQuery.LookupAppointmentStatusAsync(name);
		return Ok(status);
	}

	[HttpGet("Dates")]
	public async Task<ActionResult> GetDatesWithAppointmentsAsync(int year, int month)
	{
		var filter = new SchedulerQueryFilter
		{
			From = new DateTime(year, month, 1),
			To = new DateTime(year, month, DateTime.DaysInMonth(year, month)).Date.ToEndOfDay()
		};
		var dates = await _schedulerQuery.GetDaysScheduledAsync(filter);
		return Ok(dates);
	}

	[HttpGet("GetNotificationResult")]
	public async Task<ActionResult> GetNotificationResultAsync([FromQuery] MessageTemplateType templateType,
		[FromQuery] MessageDeliveryMethod deliveryMethod, [FromQuery] int appointmentId)
	{
		var notificationResult =
			await _schedulerService.GetNotificationResultAsync(templateType, deliveryMethod, appointmentId);
		return Ok(notificationResult);
	}

	[HttpGet("GetNotificationResults")]
	public async Task<ActionResult> GetNotificationResultsAsync([FromQuery] MessageTemplateType templateType,
		[FromQuery] MessageDeliveryMethod[] types, [FromQuery] int[] apptIds)
	{
		var notificationResults = await _schedulerService.GetNotificationResultsAsync(templateType, types, apptIds);
		return Ok(notificationResults);
	}

	/// <summary>
	///     Returns a list of all patients scheduled in the provided date range at the provided sites.
	///     If no sites (sid) are provided, all sites are queried.
	/// </summary>
	/// <param name="from"></param>
	/// <param name="to"></param>
	/// <param name="sid"></param>
	/// <returns></returns>
	[HttpGet("PatientsScheduled")]
	public async Task<ActionResult> GetPatientsScheduledAsync(DateTime from, DateTime to, int[] sid)
	{
		var patientItems = await _schedulerQuery.GetPatientsScheduledAsync(from, to, sid);
		return Ok(patientItems);
	}

	[HttpGet("PatientsScheduledToday")]
	public async Task<ActionResult> GetPatientsScheduledTodayAsync([FromQuery] int[] providerIds)
	{
		var patientItems = await _schedulerQuery.GetPatientsScheduledTodayAsync(providerIds);
		return Ok(patientItems);
	}

	[HttpGet("Resources")]
	public async Task<ActionResult> GetResourcesAsync(bool includeInactive)
	{
		var resources = await _schedulerQuery.GetResourcesAsync(includeInactive);
		return Ok(resources);
	}

	[HttpGet("IsPreviewFinished")]
	public async Task<ActionResult> IsPreviewFinished(int templateId)
	{
		var isFinished = await _patientMessagingService.IsPreviewFinishedAsync(templateId);
		return Ok(isFinished);
	}


	[HttpGet("PreviewPatientNotification")]
	public async Task<ActionResult> PreviewPatientNotificationAsync(MessageDeliveryMethod deliveryMethod,
		int templateId, int siteId, string contact)
	{
		var errors =
			await _patientMessagingService.PreviewPatientNotificationAsync(deliveryMethod, templateId, siteId, contact);

		return Ok(errors);
	}


	[HttpGet("Search")]
	public async Task<ActionResult> SearchScheduleApiAsync([FromQuery] DateTime from, [FromQuery] DateTime to,
		[FromQuery] int[] pid, [FromQuery] int[] sid, [FromQuery] int[] rid, [FromQuery] int[] spid)
	{
		var filter = new SchedulerQueryFilter
		{
			From = from,
			To = to.Date.ToEndOfDay(),
			ProviderIds = pid.ToList(),
			SiteIds = sid.ToList(),
			ResourceIds = rid.ToList(),
			SpecialtyIds = spid.ToList()
		};

		var scheduleItems = await _schedulerQuery.SearchAppointmentsAsync(filter);
		return Ok(scheduleItems);
	}

	[HttpGet("SendPatientConfirmation")]
	public async Task<ActionResult> SendPatientConfirmationAsync(int appointmentId,
		MessageDeliveryMethod deliveryMethod)
	{
		var result = await _patientMessagingService.SendConfirmationMessageAsync(appointmentId, deliveryMethod);

		return Ok(result);
	}

	[HttpGet("SendPatientVerification")]
	public async Task<ActionResult> SendPatientVerificationAsync(int appointmentId,
		MessageDeliveryMethod deliveryMethod)
	{
		var result = await _patientMessagingService.SendVerificationMessageAsync(appointmentId, deliveryMethod);
		return Ok(result);
	}

	[HttpPut("UpdateAppointment")]
	public async Task<ActionResult> UpdateAppointmentAsync([FromQuery] bool ignoreWarnings,
		[FromBody] AppointmentUpdateModel appointment)
	{
		var result = await _schedulerService.UpdateAppointmentAsync(appointment, ignoreWarnings);
		return Ok(result);
	}
}