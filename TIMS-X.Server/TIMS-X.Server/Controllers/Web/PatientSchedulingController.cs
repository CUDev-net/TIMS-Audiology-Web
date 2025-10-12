using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TIMS_X.BLL.Repositories;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.Dtos;
using TIMS_X.Server.Config;
using TIMS_X.Server.Integrations;
using TIMS_X.Server.Queries;

namespace TIMS_X.Server.Controllers.Web;

[Route("web/[controller]")]
[ApiController]
[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public class PatientSchedulingController : ControllerBase
{
	private readonly AppSettings _appSettings;
	private readonly MailgunEmailer _mailgunEmailer;
	private readonly IMessageSettingsRepository _messageSettingsRepository;
	private readonly IPatientSchedulingRepository _patientSchedulingRepository;
	private readonly IPracticeRepository _practiceRepository;
	private readonly IProviderRepository _providerRepository;
	private readonly SchedulerQuery _schedulerQuery;
	private readonly ISiteRepository _siteRepository;

	public PatientSchedulingController(IProviderRepository providerRepository,
		ISiteRepository siteRepository,
		IPatientSchedulingRepository patientSchedulingRepository,
		IPracticeRepository practiceRepository,
		MailgunEmailer mailgunEmailer,
		IOptions<AppSettings> appSettings,
		IMessageSettingsRepository messageSettingsRepository,
		SchedulerQuery schedulerQuery)
	{
		_providerRepository = providerRepository;
		_siteRepository = siteRepository;
		_patientSchedulingRepository = patientSchedulingRepository;
		_practiceRepository = practiceRepository;
		_mailgunEmailer = mailgunEmailer;
		_appSettings = appSettings.Value;
		_messageSettingsRepository = messageSettingsRepository;
		_schedulerQuery = schedulerQuery;
	}

	[HttpPost]
	public async Task<IActionResult> Create(PatientScheduledAppointmentDto patientScheduledAppointmentDto)
	{
		try
		{
			var messageSettings = await _messageSettingsRepository.Get();
			var practice = await _practiceRepository.GetPractice();
			var created = await _patientSchedulingRepository.Create(patientScheduledAppointmentDto);
			if (created != null)
			{
				var subject =
					$"Appointment Request for {patientScheduledAppointmentDto.FirstName} {patientScheduledAppointmentDto.LastName}";
				var bodyText =
					$"{created.Message}{Environment.NewLine}{created.EmailMessage}{Environment.NewLine}{created.PendingMessage}{Environment.NewLine}{created.PracticeMessage}";
				var bodyHtml = $@"
<div style=""margin: 5px; font-size: 18px;"">
    <div>{created.Message}</div>
	<div>{created.PendingMessage}</div>
    <div>{created.PracticeMessage}</div>
</div>";
				await _mailgunEmailer.EmailAsync(
					messageSettings.FromEmailAddress,
					patientScheduledAppointmentDto.Email,
					subject,
					bodyText,
					bodyHtml,
					0, practice.OfficeCode, _appSettings.Keys.ApiKey);

				var emailLog = new EmailLog
				{
					CreatedDate = DateTime.Now,
					From = messageSettings.FromEmailAddress,
					To = patientScheduledAppointmentDto.Email,
					AppointmentId = created.Appointment.Id,
					Subject = subject,
					BodyText = bodyText,
					BodyHtml = bodyHtml,
					ExceptionOccurred = false,
					ExceptionMessage = null
				};
				await _schedulerQuery.PutEmailLogAsync(emailLog);
			}

			return Ok(created);
		}
		catch (Exception ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	[HttpGet("get-practice")]
	[AllowAnonymous]
	public async Task<IActionResult> GetPractice()
	{
		var practice = await _practiceRepository.GetPracticeSummary();
		var businessRules = await _practiceRepository.GetBusinessRules();
		var dto = new PracticeDto
		{
			Locale = practice.Locale,
			Name = practice.Name,
			OnlinePatientAppointmentMessage = businessRules.OnlinePatientAppointmentMessage
		};
		return Ok(dto);
	}

	[HttpGet("get-providers")]
	[AllowAnonymous]
	public async Task<IActionResult> GetProviders()
	{
		var providers = await _providerRepository.GetSummariesForPatientScheduling();
		return Ok(providers);
	}

	[HttpGet("get-sites")]
	[AllowAnonymous]
	public async Task<IActionResult> GetSites()
	{
		var sites = await _siteRepository.GetForPatientScheduling();
		return Ok(sites);
	}

	[HttpGet("get-time-slots")]
	[AllowAnonymous]
	public async Task<IActionResult> GetTimeSlots(int providerId, int siteId, DateTime date)
	{
		var slots = await _patientSchedulingRepository.GetTimeSlots(providerId, siteId, date);
		return Ok(slots);
	}
}