using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TIMS_X.BLL.Repositories;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Utils;
using TIMS_X.DAL.Dtos;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;
using TIMS_X.Server.Utils;

namespace TIMS_X.Server.Controllers.Web;

[Route("web/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
[ApiExplorerSettings(IgnoreApi = true)]
public class SchedulerController : ControllerBase
{
	private readonly IAppointmentRepository _appointmentRepository;
	private readonly IPatientAppointmentRepository _patientAppointmentRepository;
	private readonly IPatientMessagingService _patientMessagingService;
	private readonly IScheduleRepository _scheduleRepository;

	public SchedulerController(IPatientMessagingService patientMessagingService,
		IScheduleRepository scheduleRepository,
		IAppointmentRepository appointmentRepository,
		IPatientAppointmentRepository patientAppointmentRepository)
	{
		_patientMessagingService = patientMessagingService;
		_scheduleRepository = scheduleRepository;
		_appointmentRepository = appointmentRepository;
		_patientAppointmentRepository = patientAppointmentRepository;
	}

	[HttpGet("get-appointment-items")]
	public async Task<ActionResult> GetAppointmentItems(DateTime startDate, DateTime endDate)
	{
		var userId = ClaimHelper.GetUserIdFromClaim(User);
		var items = await _appointmentRepository.GetAppointmentSummaries(startDate.Date,
			endDate.Date.AddHours(23).AddMinutes(59), userId);
		return Ok(items);
	}

	[HttpGet("get-patient-appointment-candidates")]
	public async Task<ActionResult> GetPatientAppointmentCandidates(string firstname, string lastname,
		DateTime dateOfBirth, string email, string phone)
	{
		var candidates =
			await _patientAppointmentRepository.GetPatientAppointmentCandidates(firstname, lastname, dateOfBirth, email,
				phone);

		return Ok(candidates);
	}

	[HttpGet("get-patient-appointments")]
	public async Task<ActionResult> GetPatientAppointments()
	{
		var candidates = await _patientAppointmentRepository.GetPatientAppointments();

		return Ok(candidates);
	}

	[HttpGet("get-patient-appointment-search")]
	public async Task<ActionResult> GetPatientAppointmentSearch(string name)
	{
		var candidates = await _patientAppointmentRepository.GetPatientAppointmentSearch(name);

		return Ok(candidates);
	}

	[HttpGet("get-recurring-schedule-items")]
	public async Task<ActionResult> GetRecurringScheduleItems(DateTime startDate, DateTime endDate)
	{
		var userId = ClaimHelper.GetUserIdFromClaim(User);
		var items = await _scheduleRepository.GetRecurringScheduleSummaries(startDate.Date,
			endDate.Date.AddHours(23).AddMinutes(59), userId);
		return Ok(items);
	}

	[HttpGet("get-recurring-schedule-items-for-day")]
	public async Task<ActionResult> GetRecurringScheduleItems(DateTime startDate)
	{
		var userId = ClaimHelper.GetUserIdFromClaim(User);
		var items = await _scheduleRepository.GetRecurringScheduleSummariesForDay(startDate.Date, userId);
		return Ok(items);
	}

	[HttpGet("get-schedule-items")]
	public async Task<ActionResult> GetScheduleItems(DateTime startDate, DateTime endDate)
	{
		var userId = ClaimHelper.GetUserIdFromClaim(User);
		var items = await _scheduleRepository.GetScheduleSummaries(startDate.Date,
			endDate.Date.AddHours(23).AddMinutes(59), userId);
		return Ok(items);
	}

	[HttpPost("link-patient-appointment")]
	public async Task<ActionResult> LinkPatientAppointment(PatientLinkDto patientLink)
	{
		var appointment = await _patientAppointmentRepository.LinkPatientAppointment(patientLink);

		return Ok(appointment);
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
}