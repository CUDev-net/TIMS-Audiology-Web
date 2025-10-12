using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TIMS_X.Core;
using TIMS_X.Core.Attributes;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Extensions;
using TIMS_X.Core.Models;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Config;
using TIMS_X.Server.Extensions;
using TIMS_X.Server.Hubs;
using TIMS_X.Server.Integrations;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Utils;
using Task = System.Threading.Tasks.Task;

namespace TIMS_X.Server.Services;

public class PatientMessagingService : IPatientMessagingService
{
	private readonly AppSettings _appSettings;
	private readonly AwsMessenger _awsMessenger;
	private readonly ContextHelper _contextHelper;
	private readonly CustomerQuery _customerQuery;
	private readonly MailgunEmailer _mailgunEmailer;
	private readonly PatientQuery _patientQuery;
	private readonly PatientService _patientService;
	private readonly PracticeQuery _practiceQuery;
	private readonly ProviderQuery _providerQuery;
	private readonly SchedulerQuery _schedulerQuery;
	private readonly IHubContext<SmsHub> _smsHub;
	private readonly TwilioMessenger _twilioMessenger;
	private readonly TimsUpdateQuery _updateQuery;
	private readonly UserQuery _userQuery;
	private readonly string APPOINTMENT_STATUS_CANCELLED = "Auto-Cancelled";
	private readonly string APPOINTMENT_STATUS_CONFIRMED = "Auto-Confirmed";
	private readonly string APPOINTMENT_STATUS_NEW = "New";
	private readonly string APPOINTMENT_STATUS_RESCHEDULE = "Call For Reschedule";
	private readonly string APPOINTMENT_STATUS_RESCHEDULED = "Rescheduled";

	public PatientMessagingService(
		SchedulerQuery schedulerQuery,
		PracticeQuery practiceQuery,
		ProviderQuery providerQuery,
		IConfiguration configuration,
		TwilioMessenger twilioMessenger,
		AwsMessenger awsMessenger,
		MailgunEmailer mailgunEmailer,
		PatientQuery patientQuery,
		PatientService patientService,
		UserQuery userQuery,
		TimsUpdateQuery updateQuery,
		CustomerQuery customerQuery,
		IHubContext<SmsHub> smsHub,
		ContextHelper contextHelper
	)
	{
		_schedulerQuery = schedulerQuery;
		_practiceQuery = practiceQuery;
		_patientQuery = patientQuery;
		_patientService = patientService;
		_userQuery = userQuery;
		_providerQuery = providerQuery;
		_appSettings = configuration.Get<AppSettings>();
		_twilioMessenger = twilioMessenger;
		_awsMessenger = awsMessenger;
		_mailgunEmailer = mailgunEmailer;
		_smsHub = smsHub;
		_updateQuery = updateQuery;
		_customerQuery = customerQuery;
		_contextHelper = contextHelper;
	}

	public async Task<Dictionary<int, string>> SendConfirmationMessageAsync(int appointmentId,
		MessageDeliveryMethod deliveryMethod)
	{
		var errorMessages = await _SendPatientMessagesAsync(appointmentId, appt => appt.Id == appointmentId,
			deliveryMethod,
			MessageTemplateType.AppointmentConfirmation);

		return errorMessages;
	}

	public async Task<Dictionary<int, string>> SendVerificationMessageAsync(int appointmentId,
		MessageDeliveryMethod deliveryMethod)
	{
		var errorMessages = await _SendPatientMessagesAsync(appointmentId, appt => appt.Id == appointmentId,
			deliveryMethod,
			MessageTemplateType.AppointmentVerification);

		return errorMessages;
	}

	public async Task<bool> IsPreviewFinishedAsync(int templateId)
	{
		return !await _schedulerQuery.PreviewLogExistsAsync(templateId);
	}

	public async Task<Dictionary<int, string>> PreviewPatientNotificationAsync(MessageDeliveryMethod deliveryMethod,
		int templateId, int siteId, string contact)
	{
		var errors = new Dictionary<int, string>();

		var messageTemplate = await _providerQuery.GetMessageTemplateAsync(templateId);
		if (messageTemplate == null)
		{
			errors.Add(404, $"Message template with id {templateId} could not be found.");
			return errors;
		}

		var (webServer, officeCode, qbLocale) = await _practiceQuery.GetValueAsync(
			practice => new Tuple<string, string, string>(practice.TimsServer, practice.OfficeCode, practice.QbLocale));

		var businessRules = await _practiceQuery.GetBusinessRulesAsync();

		var provider = await _providerQuery.GetProviderAsync(messageTemplate.ProviderId);
		var site = new SiteItem(await _practiceQuery.GetSiteAsync(siteId));
		var nzOffice = qbLocale.ToLower() == "nz";
		var auOffice = qbLocale.ToLower() == "au";
		var languageCode = nzOffice ? "en-NZ" : GetLanguageCodeFromEnum(messageTemplate.Language);
		var siteCityStateZip = "City, State Zip";

		if (site != null)
		{
			// Australia does not use commas in their addresses.
			if (messageTemplate.Language == LanguageEnum.EnglishAustralia)
				siteCityStateZip = $"{site.City} {site.State} {site.ZipCode}";
			else
				siteCityStateZip = $"{site.City}, {site.State} {site.ZipCode}";
		}

		var apptTime = DateTime.Now;

		if (nzOffice)
		{
			var remoteTimeZone = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
			apptTime = TimeZoneInfo.ConvertTime(DateTime.Now, remoteTimeZone);
		}
		else if (auOffice)
		{
			var remoteTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Australian Eastern Standard Time");
			apptTime = TimeZoneInfo.ConvertTime(DateTime.Now, remoteTimeZone);
		}

		var model = new PatientNotification
		{
			AppointmentId = 0,
			AppointmentStartsAt = apptTime,
			AppointmentEndsAt = apptTime.AddMinutes(30),
			AppointmentType = "Fitting",
			PatientFirstName = "Test",
			PatientLastName = "Patient",
			PatientId = 0,
			ProviderFirstName = provider.FirstName,
			ProviderLastName = provider.LastName,
			AppointmentSite = site == null ? "Main Clinic" : site.Name,
			AppointmentSiteAddress1 = site == null ? "Address 1" : site.Address1,
			AppointmentSiteAddress2 = site == null ? "Address 2" : site.Address2,
			AppointmentSiteCityStateZip = siteCityStateZip,
			AppointmentSitePhone = site == null ? string.Empty : site.Phone,
			Language = messageTemplate.Language,
			DateTimeFormat = new CultureInfo(languageCode, false).DateTimeFormat,
			ConfirmationLink =
				"<ConfirmationLink>", //string.Format(responseUrl ?? "#", (int)PatientNotificationResponse.Confirm),
			CancelLink = "<CancelLink>", //string.Format(responseUrl ?? "#", (int)PatientNotificationResponse.Cancel),
			CallForRescheduleLink =
				"<RescheduleLink>", //string.Format(responseUrl ?? "#", (int)PatientNotificationResponse.Reschedule),
			DigitalPatientIntakeLink =
				"<DigitalPatientIntakeLink>" //string.Format(responseUrl ?? "#", (int)PatientNotificationResponse.Reschedule),
		};

		var messageSettings = await _practiceQuery.GetMessageSettingsAsync();

		string errorMessage = null;
		switch (deliveryMethod)
		{
			case MessageDeliveryMethod.Email:
				errorMessage = _CanSendEmailNotification(contact, messageTemplate, NotificationStatus.NothingSent,
					messageSettings, null);
				if (string.IsNullOrEmpty(errorMessage))
					await _SendEmailNotificationAsync(contact, messageTemplate, messageSettings,
						model, webServer, officeCode, true, null);
				else
					errors[0] = errorMessage;
				break;
			case MessageDeliveryMethod.Sms:
				errorMessage = _CanSendSmsNotification(contact, messageTemplate, NotificationStatus.NothingSent,
					messageSettings, null);
				if (string.IsNullOrEmpty(errorMessage))
					await _SendSmsNotificationAsync(contact, messageTemplate, messageSettings,
						model, officeCode, true, null, webServer, qbLocale, businessRules.UseAmazonMessaging);
				else
					errors[0] = errorMessage;
				break;
			case MessageDeliveryMethod.Voice:
				errorMessage = _CanSendVoiceNotification(contact, messageTemplate, NotificationStatus.NothingSent,
					messageSettings, null);
				if (string.IsNullOrEmpty(errorMessage))
					await _SendVoiceNotificationAsync(contact, messageTemplate, messageSettings,
						model, officeCode, true, null);
				else
					errors[0] = errorMessage;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(deliveryMethod), deliveryMethod, null);
		}

		return errors;
	}

	public async Task<CallScriptTokens> GetCallScriptTokensAsync(string officeCode,
		string key,
		string callSid)
	{
		var callLog = await _schedulerQuery.GetCallLogAsync(callSid);
		if (callLog == null) return null;

		var callbackBaseUrl = await _practiceQuery.GetValueAsync(p => p.WebServer);
		var callScriptTokens = await GetCallScriptTokensAsync(callLog, callbackBaseUrl, officeCode, key);
		return callScriptTokens;
	}

	public async Task<MessageTemplate> GetMessageTemplateFromCallSidAsync(string callSid)
	{
		var template = await _providerQuery.GetMessageTemplateFromCallLogAsync(callSid);
		return template;
	}

	public async Task<MessageTemplate> GetMessageTemplateFromSmsLogAsync(string phoneNumber)
	{
		var template = await _providerQuery.GetMessageTemplateFromSmsLogAsync(phoneNumber);
		return template;
	}

	public async Task<MessageTemplate> GetMessageTemplateAsync(int id)
	{
		var template = await _providerQuery.GetMessageTemplateAsync(id);
		return template;
	}

	public async Task<MessageTemplate> GetMessageTemplateFromEmailLogAsync(int logId)
	{
		var template = await _providerQuery.GetMessageTemplateFromEmailLogAsync(logId);
		return template;
	}

	public async Task<Tuple<PatientNotificationResponse, MessageTemplate, int, int>>
		HandlePatientSmsNotificationResponseAsync(AwsSmsNotification smsResponse)
	{
		return await _HandlePatientSmsNotificationResponseAsync(smsResponse.OriginationNumber, smsResponse.MessageBody,
			smsResponse.InboundMessageId, "received");
	}

	public async Task<Tuple<PatientNotificationResponse, MessageTemplate, int, int>>
		HandlePatientSmsNotificationResponseAsync(TwilioSmsResponse smsResponse)
	{
		return await _HandlePatientSmsNotificationResponseAsync(smsResponse.From, smsResponse.Body, smsResponse.SmsSid,
			smsResponse.SmsStatus,
			smsResponse.MediaContentType0, smsResponse.MediaUrl0,
			smsResponse.MediaContentType1, smsResponse.MediaUrl1,
			smsResponse.MediaContentType2, smsResponse.MediaUrl2,
			smsResponse.MediaContentType3, smsResponse.MediaUrl3);
	}

	public async Task<Tuple<PatientNotificationResponse, MessageTemplate>> HandlePatientVoiceNotificationResponseAsync(
		TwilioCallResponse callResponse)
	{
		var notificationResponse = PatientNotificationResponse.Unknown;
		// get the response as an integer value
		if (int.TryParse(callResponse.Digits.Trim(), out var integerResponse))
			notificationResponse = (PatientNotificationResponse)integerResponse;
		// Retrieve the log entry for this call
		var callLog = await _schedulerQuery.GetCallLogAsync(callResponse.CallSid);
		if (callLog == null) throw new Exception($"No call log found for call sid '{callResponse.CallSid}'");

		var template = callLog.MessageTemplate;

		if (!callLog.IsPreview)
		{
			// We add a tracking row every time the status changes. Important for customer billing
			var voiceTracking = new VoiceCallTracking
			{
				VoiceCallLogId = callLog.Id,
				Status = callResponse.CallStatus,
				DigitsPressed = callResponse.Digits,
				CreatedDate = DateTime.Now
			};
			await _schedulerQuery.PutCallTrackingAsync(voiceTracking);
			var officeCode = await _practiceQuery.GetValueAsync(x => x.OfficeCode);
			await _ProcessNotificationResponse(notificationResponse, callLog.AppointmentId, officeCode);
		}


		return new Tuple<PatientNotificationResponse, MessageTemplate>(notificationResponse, template);
	}

	public async Task<Tuple<MessageTemplate, int, bool, int>> HandlePatientEmailNotificationResponseAsync(
		PatientNotificationResponse response, int logId)
	{
		// Retrieve the log entry for this email
		var emailLog = await _schedulerQuery.GetEmailLogAsync(logId);
		if (emailLog == null) throw new Exception($"No email log found with id '{logId}'");

		var appointmentId = emailLog.AppointmentId;
		var siteId = 0;
		var wasModified = false;
		var patientId = 0;
		if (appointmentId > 0)
		{
			var appt = await _schedulerQuery.GetAppointmentAsync(appointmentId, false);
			if (appt != null) patientId = appt.PatientId;
			var officeCode = await _practiceQuery.GetValueAsync(x => x.OfficeCode);
			wasModified = await _ProcessNotificationResponse(response, appointmentId, officeCode);
			siteId = await _schedulerQuery.GetSiteIdForAppointmentAsync(appointmentId);
		}

		return new Tuple<MessageTemplate, int, bool, int>(emailLog.MessageTemplate, siteId, wasModified, patientId);
	}

	public async Task UpdateCallStatusAsync(string callSid, string callStatus, string digitsPressed)
	{
		var callCompleted = callStatus == "completed";
		var callLog = await _schedulerQuery.GetCallLogAsync(callSid);
		if (callLog != null)
			try
			{
				if (callLog.IsPreview)
				{
					if (callCompleted) await _schedulerQuery.DeleteCallLogAsync(callSid);
				}
				else
				{
					var tracking = new VoiceCallTracking
					{
						CreatedDate = DateTime.Now,
						DigitsPressed = digitsPressed,
						Status = callStatus,
						VoiceCallLogId = callLog.Id
					};
					await _schedulerQuery.PutCallTrackingAsync(tracking);
				}
			}
			catch (Exception ex)
			{
				callLog.ExceptionOccurred = true;
				callLog.ExceptionMessage = ex.Message;
				await _schedulerQuery.PutCallLogAsync(callLog);
				throw;
			}
	}

	public async Task UpdateSmsStatusAsync(TwilioSmsStatus status)
	{
		if (status != null)
		{
			var smsLog = await _schedulerQuery.GetSmsLogByIdentifierAsync(status.SmsSid);
			if (smsLog != null)
			{
				var tracking = new SmsTracking
				{
					CreatedDate = DateTime.Now,
					Status = status.SmsStatus,
					SmsLogId = smsLog.Id
				};
				await _schedulerQuery.PutSmsTrackingAsync(tracking);
			}
		}
	}

	public async Task SendSmsReplyAsync(string to, string message, int patientId, int userId, int templateId,
		int appointmentId)
	{
		var messageSettings = await _practiceQuery.GetMessageSettingsAsync();
		var (officeCode, serverUrl, qbLocale) = await _practiceQuery.GetValueAsync(p =>
			new Tuple<string, string, string>(p.OfficeCode, p.TimsServer, p.QbLocale));
		var businessRules = await _practiceQuery.GetBusinessRulesAsync();
		var formattedTo = to.ToE164Format(qbLocale);
		if (message.Contains("{DigitalIntakeFormLink}"))
		{
			string intakeLink = null;

			if (patientId > 0)
			{
				var (success, link) =
					await _patientService.GetFormLinkAsync(PatientFormTypeEnum.Intake, patientId, userId);
				// Append a space, otherwise any text after the link will be concatenated and the link will fail.
				if (success) intakeLink = link + " ";
			}

			message = message.Replace("{DigitalIntakeFormLink}", intakeLink);
		}

		var createdDate = await _ConvertDateToCustomerTimeZoneAsync(officeCode, DateTime.Now);
		var smsLog = new SmsLog();
		smsLog.MessageTemplateId = templateId;
		smsLog.From = messageSettings.FromSmsNumber;
		smsLog.To = formattedTo;
		smsLog.Body = message;
		smsLog.AppointmentId = appointmentId;
		smsLog.PatientId = patientId;
		smsLog.CreatedDate = createdDate;
		smsLog.IsPreview = false;
		smsLog.ExceptionOccurred = false;
		smsLog.ExceptionMessage = null;
		smsLog.CreatedUserId = userId;

		try
		{
			if (businessRules.UseAmazonMessaging)
				smsLog.Identifier = await _awsMessenger.SendSmsAsync(messageSettings.FromSmsNumber,
					formattedTo, message, officeCode, _appSettings.Keys.ApiKey, serverUrl, qbLocale);
			else
				smsLog.Identifier = await _twilioMessenger.SendSmsAsync(messageSettings.FromSmsNumber,
					formattedTo, message, officeCode, _appSettings.Keys.ApiKey, serverUrl, qbLocale);
		}
		catch (Exception ex)
		{
			// Add exception info to the log entry.
			smsLog.ExceptionOccurred = true;
			smsLog.ExceptionMessage = ex.GetBaseException().Message;
		}
		finally
		{
			await _schedulerQuery.PutSmsLogAsync(smsLog);
		}
	}

	public async Task<int> GetConversationHistoryMessageCountAsync(int userId, int patientId)
	{
		return await _patientQuery.GetConversationHistoryMessageCountAsync(userId, patientId);
	}

	public async Task<List<Tuple<int, int>>> GetConversationHistoryMessageCountAllAsync(int patientId)
	{
		return await _patientQuery.GetConversationHistoryMessageCountAllAsync(patientId);
	}

	public async Task<List<Message>> GetConversationHistoryAsync(int userId, int patientId, DateTime? cutoffDate)
	{
		return await _patientQuery.GetConversationHistoryAsync(userId, patientId, cutoffDate);
	}

	public async Task<List<Message>> GetConversationHistoryAsync(int userId, string phoneNumber, DateTime? cutoffDate)
	{
		return await _patientQuery.GetConversationHistoryAsync(userId, phoneNumber, cutoffDate);
	}

	public async Task<List<Conversation>> GetAllConversationsAsync()
	{
		return await _patientQuery.GetAllConversationsAsync();
	}

	public async Task<List<Conversation>> GetAllConversationsPagedAsync(int page, int pageSize)
	{
		return await _patientQuery.GetAllConversationsPagedAsync(page, pageSize);
	}

	public async Task<int> GetAllConversationsCountAsync()
	{
		return await _patientQuery.GetAllConversationsCountAsync();
	}

	public async Task SendPatientMessagesAsync(PatientMessageModel model)
	{
		if (string.IsNullOrEmpty(model.Message))
			return;

		var messageSettings = await _practiceQuery.GetMessageSettingsAsync();
		var (officeCode, serverUrl, qbLocale) = await _practiceQuery.GetValueAsync(p =>
			new Tuple<string, string, string>(p.OfficeCode, p.TimsServer, p.QbLocale));
		var businessRules = await _practiceQuery.GetBusinessRulesAsync();
		if (model.DeliveryMethod == MessageDeliveryMethod.All || model.DeliveryMethod == MessageDeliveryMethod.Email)
			await _mailgunEmailer.EmailAsync(model, officeCode, _appSettings.Keys.ApiKey, messageSettings);

		if (model.DeliveryMethod == MessageDeliveryMethod.All || model.DeliveryMethod == MessageDeliveryMethod.Sms)
		{
			var createdDate = await _ConvertDateToCustomerTimeZoneAsync(officeCode, DateTime.Now);
			var smsLog = new SmsLog();
			smsLog.MessageTemplateId = 0;
			smsLog.From = messageSettings.FromSmsNumber;
			smsLog.To = model.PhoneNumber.ToE164Format(qbLocale);
			smsLog.Body = model.Message;
			smsLog.AppointmentId = 0;
			smsLog.PatientId = model.PatientId;
			smsLog.CreatedDate = createdDate;
			smsLog.IsPreview = false;
			smsLog.ExceptionOccurred = false;
			smsLog.ExceptionMessage = null;
			smsLog.CreatedUserId = _contextHelper.CurrentUser.Id;

			try
			{
				if (businessRules.UseAmazonMessaging)
					smsLog.Identifier = await _awsMessenger.SendSmsAsync(messageSettings.FromSmsNumber,
						model.PhoneNumber, model.Message, officeCode, _appSettings.Keys.ApiKey, serverUrl, qbLocale);
				else
					smsLog.Identifier = await _twilioMessenger.SendSmsAsync(messageSettings.FromSmsNumber,
						model.PhoneNumber, model.Message, officeCode, _appSettings.Keys.ApiKey, serverUrl, qbLocale);
			}
			catch (Exception ex)
			{
				// Add exception info to the log entry.
				smsLog.ExceptionOccurred = true;
				smsLog.ExceptionMessage = ex.GetBaseException().Message;
			}
			finally
			{
				await _schedulerQuery.PutSmsLogAsync(smsLog);
			}
		}
	}

	public async Task HandleAwsSmsResponseAsync(AwsSmsNotification smsNotification)
	{
		await Task.Run(() => { });
	}

	private string _CanSendEmailNotification(string to, MessageTemplate messageTemplate,
		NotificationStatus notificationStatus, MessageSettings messageSettings, EmailLog emailLog)
	{
		if (string.IsNullOrWhiteSpace(to))
			return "Recipient email address is missing or invalid.";

		if (to.IndexOf(',') > -1)
			return "Multiple recipients not allowed.";

		if (messageTemplate == null)
			return "Message template is missing.";

		if (string.IsNullOrWhiteSpace(messageSettings.FromEmailAddress))
			return "From email address is missing.";

		if (!messageSettings.IsEmailEnabled)
			return "Outbound email is disabled.";

		if (string.IsNullOrWhiteSpace(_appSettings.Keys.TwilioAccountSid))
			return "Twilio account sid is missing.";

		if (string.IsNullOrWhiteSpace(_appSettings.Keys.TwilioAuthToken))
			return "Twilio auth token is missing.";

		if (!messageTemplate.IsEmailEnabled)
			return "Emailing is disabled for this notification template.";

		if (messageTemplate.MessageType == MessageTemplateType.AppointmentConfirmation.ToString())
		{
			if (notificationStatus.HasFlag(NotificationStatus.EmailConfirmationSent))
			{
				if (emailLog != null && emailLog.ExceptionOccurred)
					return null;
				return "An email confirmation was already sent for this appointment.";
			}
		}
		else if (notificationStatus.HasFlag(NotificationStatus.EmailReminderSent))
		{
			if (emailLog != null && emailLog.ExceptionOccurred)
				return null;
			return "An email verification was already sent for this appointment.";
		}

		return null;
	}

	private string _CanSendSmsNotification(string to, MessageTemplate messageTemplate,
		NotificationStatus notificationStatus, MessageSettings messageSettings, SmsLog smsLog)
	{
		// Check constraints
		if (string.IsNullOrWhiteSpace(to))
			return "Recipient phone number is missing or invalid.";
		if (messageTemplate == null)
			return "Message template is missing.";
		// Check constraints
		if (string.IsNullOrWhiteSpace(messageSettings.FromSmsNumber))
			return "From sms number is missing.";
		// Short circuits
		if (!messageSettings.IsSmsEnabled)
			return "Outbound sms is disabled.";

		if (string.IsNullOrWhiteSpace(_appSettings.Keys.TwilioAccountSid))
			return "Twilio account sid is missing.";

		if (string.IsNullOrWhiteSpace(_appSettings.Keys.TwilioAuthToken))
			return "Twilio auth token is missing.";

		if (!messageTemplate.IsSmsEnabled)
			return "Sms Messaging is disabled for this notification template.";

		if (smsLog != null && !smsLog.ExceptionOccurred)
		{
			if (messageTemplate.MessageType == MessageTemplateType.AppointmentConfirmation.ToString())
			{
				if (notificationStatus.HasFlag(NotificationStatus.SmsConfirmationSent))
					return "An sms confirmation was already sent for this appointment.";
			}
			else if (notificationStatus.HasFlag(NotificationStatus.SmsReminderSent))
			{
				return "An sms verification was already sent for this appointment.";
			}
		}


		return null;
	}

	private string _CanSendVoiceNotification(string to, MessageTemplate messageTemplate,
		NotificationStatus notificationStatus, MessageSettings messageSettings, VoiceCallLog callLog)
	{
		// Check constraints
		if (string.IsNullOrWhiteSpace(to))
			return "Recipient phone number is missing or invalid.";
		// Check constraints
		if (string.IsNullOrWhiteSpace(messageSettings.FromPhoneNumber))
			return
				"'From' phone number is missing in setup. Please contact TIMS Support to configure patient notifications.";
		if (messageTemplate == null)
			return "Message template is missing from the database. Please contact TIMS Support if this error persists.";

		// Short circuits
		if (!messageSettings.IsVoiceEnabled)
			return
				"Outbound calls are disabled in setup. Please contact TIMS Support to configure patient notifications.";

		if (string.IsNullOrWhiteSpace(_appSettings.Keys.TwilioAccountSid))
			return "Twilio account sid is missing.  Please contact TIMS Support to configure patient notifications.";

		if (string.IsNullOrWhiteSpace(_appSettings.Keys.TwilioAuthToken))
			return "Twilio auth token is missing.  Please contact TIMS Support to configure patient notifications.";

		if (!messageTemplate.IsVoiceEnabled)
			return "Voice notifications are disabled for this template.";

		if (messageTemplate.MessageType == MessageTemplateType.AppointmentConfirmation.ToString())
		{
			if (notificationStatus.HasFlag(NotificationStatus.VoiceConfirmationSent))
			{
				// If the last notification errored out, allow it to be sent again.
				if (callLog != null && callLog.ExceptionOccurred) return null;
				return "A voice confirmation was already sent for this appointment.";
			}
		}
		else if (notificationStatus.HasFlag(NotificationStatus.VoiceReminderSent))
		{
			if (callLog == null) return "Message marked as sent but no log entry exists.";
			// If the last notification errored out, allow it to be sent again.
			if (callLog.ExceptionOccurred) return null;
			return "A voice verification was already sent for this appointment.";
		}

		return null;
	}

	private async Task<DateTime> _ConvertDateToCustomerTimeZoneAsync(string officeCode, DateTime date)
	{
		try
		{
			var timeZone = await _customerQuery.GetTimeZoneAsync(officeCode);
			if (timeZone.HasValue)
			{
				TimeZoneInfo timeZoneInfo;
				if (timeZone.Value == TimsTimeZone.Greenwich)
					timeZoneInfo = TimeZoneInfo.Utc;
				else
					timeZoneInfo =
						TimeZoneInfo.FindSystemTimeZoneById(EnumUtilities.GetDescriptionFromEnum(timeZone.Value));
				date = TimeZoneInfo.ConvertTime(date, TimeZoneInfo.Local, timeZoneInfo);
			}
		}
		catch
		{
			// do nothing for now
		}

		return date;
	}


	private string _GenerateResponseLink(string baseUrl, string officeCode, PatientNotificationResponse responseCode,
		int logId, string linkText)
	{
		if (string.IsNullOrEmpty(baseUrl))
			return null;
		var parameters = new EmailResponseParameters
		{
			LogId = logId,
			ResponseCode = responseCode
		};

		var paramsJson = JsonConvert.SerializeObject(parameters);
		var encrypted = CryptographyHelper.Encrypt(paramsJson, _appSettings.Keys.ImagingKey);
		var encryptedBytes = Encoding.UTF8.GetBytes(encrypted);
		var encryptedBase64 = Convert.ToBase64String(encryptedBytes);

		var uriBuilder = new UriBuilder(baseUrl)
			.AppendToPath("ConfirmAppointment");

		uriBuilder.Query = new ParameterBuilder()
			.Add("officeCode", officeCode)
			.Add("response", encryptedBase64)
			.ToString();
		return $"<a href=\"{uriBuilder}\">{linkText}</a>";
	}

	/// <summary>
	///     Try to find language enum matching languageCode. Return American English if no match.
	/// </summary>
	/// <param name="languageCode"></param>
	/// <returns></returns>
	private LanguageEnum _GetLanguageEnum(string languageCode)
	{
		var languageList = Enum.GetValues(typeof(LanguageEnum)).Cast<LanguageEnum>().ToList();
		var language = languageList.FirstOrDefault(l => GetLanguageCodeFromEnum(l) == languageCode);
		return language == LanguageEnum.None ? LanguageEnum.English : language;
	}

	/// <summary>
	/// </summary>
	/// <param name="templateType"></param>
	/// <param name="messageTemplate"></param>
	/// <param name="officeCode"></param>
	/// <param name="key"></param>
	/// <param name="model"></param>
	/// <returns></returns>
	private List<string> _GetSmsBodyChunked(
		MessageTemplateType templateType,
		MessageTemplate messageTemplate,
		string officeCode,
		string key,
		PatientNotification model)
	{
		var tokenReplacements = TokenTransformer.GetTokenReplacements(model);
		var smsBody = TokenTransformer.TransformTokens(
			messageTemplate.GetSmsBody(templateType), tokenReplacements);
		var chunkedMessage = smsBody.SplitByLength(160);
		return chunkedMessage;
	}

	private async Task<Tuple<PatientNotificationResponse, MessageTemplate, int, int>>
		_HandlePatientSmsNotificationResponseAsync(string from, string message, string identifier, string status,
			string mediaContentType0 = null,
			string mediaUrl0 = null,
			string mediaContentType1 = null,
			string mediaUrl1 = null,
			string mediaContentType2 = null,
			string mediaUrl2 = null,
			string mediaContentType3 = null,
			string mediaUrl3 = null)
	{
		var notificationResponse = PatientNotificationResponse.Unknown;
		// Retrieve the log entry for this call
		var smsLog = await _schedulerQuery.GetSmsLogAsync(from);
		var officeCode = await _practiceQuery.GetValueAsync(x => x.OfficeCode);
		var messageSettings = await _practiceQuery.GetMessageSettingsAsync();
		var patientId = 0;

		Appointment appt = null;

		// replies to verifications aren't allowed so set smsLog to null
		if (smsLog != null && smsLog.MessageTemplate != null &&
		    smsLog.MessageTemplate.TemplateType == MessageTemplateType.AppointmentVerification) smsLog = null;

		if (smsLog != null && smsLog.AppointmentId > 0)
		{
			// check if appointment has already been replied to. If that is the case, this message should not be treated as a notification response.
			appt = await _schedulerQuery.GetAppointmentAsync(smsLog.AppointmentId, true);
			if (appt.AppointmentStatus != null && appt.AppointmentStatus.Name != APPOINTMENT_STATUS_NEW &&
			    appt.AppointmentStatus.Name != APPOINTMENT_STATUS_RESCHEDULED)
			{
				// Check if patient sent a 1, 2 or 3. If they did, ignore it in this case because the scenario is:
				// 1) The last message sent to the patient was an appt notification
				// 2) The appt was responded to already
				// 3) They can't be responding to a question from the office since the last message was a notification
				// This means this message is almost certainly a duplicate, sent on accident.
				if (!string.IsNullOrWhiteSpace(message))
				{
					var trimmedMessage = message.Trim();
					if (trimmedMessage == "1" || trimmedMessage == "2" || trimmedMessage == "3")
						return new Tuple<PatientNotificationResponse, MessageTemplate, int, int>(
							PatientNotificationResponse.Unknown, null, 0, 0);
				}

				smsLog = null;
			}
		}

		// If smsLog is null, the message is not a reply to a previous message. We don't even know if the patient is in TIMS.
		if (smsLog == null)
		{
			// find a patient with matching phone number...
			var patient = await _patientQuery.LookupPatientAsync(from);
			int? providerId = null;
			if (patient != null)
			{
				patientId = patient.Id;
				providerId = patient.ProviderId;
			}

			var defaultMessageReceiverId = await _userQuery.GetDefaultMessageReceiverAsync();
			var createdDate = await _ConvertDateToCustomerTimeZoneAsync(officeCode, DateTime.Now);
			// create a stub sms log
			smsLog = new SmsLog();
			smsLog.MessageTemplateId = 0;
			smsLog.From = messageSettings.FromPhoneNumber;
			smsLog.To = from;
			smsLog.Body = null;
			smsLog.AppointmentId = 0;
			smsLog.PatientId = patientId;
			smsLog.CreatedDate = createdDate;
			smsLog.IsPreview = false;
			smsLog.ExceptionOccurred = false;
			smsLog.ExceptionMessage = null;
			smsLog.CreatedUserId = defaultMessageReceiverId;

			await _patientQuery.PutSmsLogAsync(smsLog);

			createdDate = await _ConvertDateToCustomerTimeZoneAsync(officeCode, DateTime.Now);

			// create the reply
			var smsReply = new SmsReply
			{
				CreatedDate = createdDate,
				From = from,
				Body = message,
				Identifier = identifier ?? string.Empty,
				SmsLogId = smsLog.Id
			};

			await _patientQuery.PutSmsReplyAsync(smsReply);

			// notify user

			var userId = $"{officeCode}-{defaultMessageReceiverId}".ToLower();

			if (SmsHub.IsUserConnected(userId))
			{
				await _smsHub.Clients.User(userId).SendAsync("ReceiveMessage", patientId, from, smsReply.Body);
			}
			else if (!await _practiceQuery.AlertExistsAsync(smsLog.CreatedUserId.Value, AlertTypeEnum.MessageReceived,
				         smsLog.PatientId))
			{
				if (smsLog.PatientId > 0)
				{
					var patientDetails = await _patientQuery.GetPatientAsync(smsLog.PatientId);
					from = patientDetails.Name;
				}

				var alert = new Alert
				{
					CreatedUserId = 0,
					CreatedDate = DateTime.Now,
					DueDate = DateTime.Now,
					Name = EnumUtilities.GetDescriptionFromEnum(AlertTypeEnum.MessageReceived) + " Alert",
					Description = $"New message from {from}",
					AlertUserId = smsLog.CreatedUserId.Value,
					AlertObjectId = smsLog.PatientId,
					AlertType = AlertTypeEnum.MessageReceived
				};
				await _practiceQuery.PutAlertAsync(alert);
			}

			return new Tuple<PatientNotificationResponse, MessageTemplate, int, int>(
				PatientNotificationResponse.Unknown, null, 0, 0);
		}

		patientId = smsLog.PatientId;

		if (!smsLog.IsPreview)
		{
			// We add a tracking row every time the status changes. Important for customer billing
			var smsTracking = new SmsTracking
			{
				SmsLogId = smsLog.Id,
				Status = status,
				CreatedDate = DateTime.Now
			};
			await _schedulerQuery.PutSmsTrackingAsync(smsTracking);


			if (smsLog.AppointmentId == 0 && smsLog.CreatedUserId.HasValue)
			{
				if (patientId == 0)
				{
					// find a patient with matching phone number...
					var patient = await _patientQuery.LookupPatientAsync(from);

					if (patient != null)
					{
						smsLog.PatientId = patient.Id;
						await _patientQuery.UpdateUnknownLogEntriesAsync(from, patient.Id);
						patientId = patient.Id;
					}
				}

				var media = new List<Tuple<string, string>>();

				if (!string.IsNullOrEmpty(mediaContentType0) && !string.IsNullOrEmpty(mediaUrl0))
					media.Add(new Tuple<string, string>(mediaContentType0, mediaUrl0));
				if (!string.IsNullOrEmpty(mediaContentType1) && !string.IsNullOrEmpty(mediaUrl1))
					media.Add(new Tuple<string, string>(mediaContentType1, mediaUrl1));
				if (!string.IsNullOrEmpty(mediaContentType2) && !string.IsNullOrEmpty(mediaUrl2))
					media.Add(new Tuple<string, string>(mediaContentType2, mediaUrl2));
				if (!string.IsNullOrEmpty(mediaContentType3) && !string.IsNullOrEmpty(mediaUrl3))
					media.Add(new Tuple<string, string>(mediaContentType3, mediaUrl3));

				var body = message;
				if (body == null)
					body = string.Empty;

				if (media.Any())
				{
					var supportsMedia = false;
					var version = await _updateQuery.GetCurrentVersionAsync();
					if (!string.IsNullOrEmpty(version))
					{
						var components = version.Split(".");
						if (components.Length == 4)
							if (int.TryParse(components[2], out var versionMajor) &&
							    int.TryParse(components[3], out var versionMinor))
							{
#if DEBUG
								if ((versionMajor == 7 && versionMinor > 31600) || versionMajor > 7)
#else
                                    if ((versionMajor == 7 && versionMinor > 31600) || versionMajor > 7)
#endif
									supportsMedia = true;
							}
					}

					if (supportsMedia)
					{
						var sb = new StringBuilder("---------\nMEDIA\n---------\n");

						foreach (var item in media)
							// Item1 = ContentType, Item2 = Url
							sb.Append($"{item.Item1}|{item.Item2}\n");

						if (string.IsNullOrEmpty(body))
							body = sb.ToString().TrimEnd('\n');
						else
							body += "\n" + sb.ToString().TrimEnd('\n');
					}
				}

				var smsReply = new SmsReply
				{
					CreatedDate = DateTime.Now,
					From = from,
					Body = body,
					Identifier = identifier,
					SmsLogId = smsLog.Id
				};
				await _patientQuery.PutSmsReplyAsync(smsReply);

				var userId = $"{officeCode}-{smsLog.CreatedUserId.Value}".ToLower();

				if (SmsHub.IsUserConnected(userId))
				{
					await _smsHub.Clients.User(userId)
						.SendAsync("ReceiveMessage", smsLog.PatientId, from, smsReply.Body);
				}
				else
				{
					var alertExists = await _practiceQuery.AlertExistsAsync(smsLog.CreatedUserId.Value,
						AlertTypeEnum.MessageReceived, smsLog.PatientId, smsLog.PatientId > 0 ? null : from);

					if (!alertExists)
					{
						string fromIdentifier;
						if (smsLog.PatientId > 0)
						{
							var patientDetails = await _patientQuery.GetPatientAsync(smsLog.PatientId);
							fromIdentifier = patientDetails.Name;
						}
						else
						{
							fromIdentifier = from;
						}

						var alert = new Alert
						{
							CreatedUserId = 0,
							CreatedDate = DateTime.Now,
							DueDate = DateTime.Now,
							Name = EnumUtilities.GetDescriptionFromEnum(AlertTypeEnum.MessageReceived) + " Alert",
							Description = $"New message from {fromIdentifier}",
							AlertUserId = smsLog.CreatedUserId.Value,
							AlertObjectId = smsLog.PatientId,
							AlertType = AlertTypeEnum.MessageReceived
						};
						await _practiceQuery.PutAlertAsync(alert);
					}
				}
			}
			else if (smsLog.AppointmentId > 0)
			{
				// get the response as an integer value
				if (int.TryParse(message.Trim(), out var integerResponse))
				{
					notificationResponse = (PatientNotificationResponse)integerResponse;

					switch (notificationResponse)
					{
						case PatientNotificationResponse.Cancel:
							if (!smsLog.MessageTemplate.AllowSmsCancel)
								notificationResponse = PatientNotificationResponse.Unknown;
							break;
						case PatientNotificationResponse.Confirm:
							if (!smsLog.MessageTemplate.AllowSmsConfirm)
								notificationResponse = PatientNotificationResponse.Unknown;
							break;
						case PatientNotificationResponse.Repeat:
							notificationResponse = PatientNotificationResponse.Unknown;
							break;
						case PatientNotificationResponse.Reschedule:
							if (!smsLog.MessageTemplate.AllowSmsReschedule)
								notificationResponse = PatientNotificationResponse.Unknown;
							break;
						case PatientNotificationResponse.Unknown:
							break;
					}
				}

				appt ??= await _schedulerQuery.GetAppointmentAsync(smsLog.AppointmentId, false);
				patientId = appt.PatientId;
				await _ProcessNotificationResponse(notificationResponse, smsLog.AppointmentId, officeCode);
			}
		}

		return new Tuple<PatientNotificationResponse, MessageTemplate, int, int>(notificationResponse,
			smsLog.MessageTemplate, patientId, smsLog.AppointmentId);
	}

	private async Task<bool> _ProcessNotificationResponse(PatientNotificationResponse notificationResponse,
		int appointmentId, string officeCode)
	{
		// Check for valid user input. Return immediately if user did not enter 1, 2, or 3
		string appointmentStatusName = null;
		switch (notificationResponse)
		{
			case PatientNotificationResponse.Unknown:
				return true;
			case PatientNotificationResponse.Confirm: // CONFIRM
				appointmentStatusName = APPOINTMENT_STATUS_CONFIRMED;
				break;
			case PatientNotificationResponse.Reschedule: // RESCHEDULE
				appointmentStatusName = APPOINTMENT_STATUS_RESCHEDULE;
				break;
			case PatientNotificationResponse.Cancel: // CANCEL
				appointmentStatusName = APPOINTMENT_STATUS_CANCELLED;
				break;
			case PatientNotificationResponse.Repeat: // REPEAT MESSAGE, DO NO MORE PROCESSING
				return false;
			default:
				throw new Exception($"Invalid patient response:'{notificationResponse}'");
				;
		}


		// Fetch target appointment
		var appointment = await _schedulerQuery.GetAppointmentAsync(appointmentId, true);

		if (appointment == null) throw new Exception($"No appointment found with id '{appointmentId}'");

		if (appointmentStatusName == null)
		{
			throw new Exception($"Invalid patient response:'{notificationResponse}'");
			;
		}

		var appointmentStatus = await _schedulerQuery.LookupAppointmentStatusAsync(appointmentStatusName);

		var updatedDate = await _ConvertDateToCustomerTimeZoneAsync(officeCode, DateTime.Now);


		if (appointmentStatus == null)
		{
			appointmentStatus = new AppointmentStatus
			{
				Name = appointmentStatusName,
				Show = appointmentStatusName != APPOINTMENT_STATUS_CANCELLED,
				Protected = true,
				UpdatedDate = updatedDate
			};

			await _schedulerQuery.PutAppointmentStatusAsync(appointmentStatus);
		}

		if (string.Equals(appointment.AppointmentStatus.Name, APPOINTMENT_STATUS_NEW,
			    StringComparison.CurrentCultureIgnoreCase) ||
		    string.Equals(appointment.AppointmentStatus.Name, APPOINTMENT_STATUS_RESCHEDULED,
			    StringComparison.CurrentCultureIgnoreCase)) // is appt new or rescheduled?
		{
			appointment.AppointmentStatusId = appointmentStatus.Id;
			if (string.IsNullOrWhiteSpace(appointment.Notes))
				appointment.Notes = string.Empty;
			else
				appointment.Notes += Environment.NewLine;

			appointment.Notes +=
				$"Updated status to {appointmentStatusName} on {updatedDate.ToShortDateString()} at {updatedDate.ToShortTimeString()}";
			appointment.UpdatedUserId = 0;
			appointment.UpdatedDate = updatedDate;

			await _schedulerQuery.PutAppointmentAsync(appointment);
			return true;
		}

		if (string.Equals(appointment.AppointmentStatus.Name, appointmentStatusName,
			    StringComparison.CurrentCultureIgnoreCase)) return true;

		return false;
	}

	private async Task _SendEmailNotificationAsync(string to, MessageTemplate messageTemplate,
		MessageSettings messageSettings, PatientNotification notification, string baseUrl, string officeCode,
		bool isPreview, EmailLog emailLog)
	{
		var from = messageSettings.FromEmailAddress;
		var tokenReplacements = TokenTransformer.GetTokenReplacements(notification);
		var transformedSubject = TokenTransformer.TransformTokens(messageTemplate.EmailSubject, tokenReplacements);
		var transformedBodyText = TokenTransformer.TransformTokens(messageTemplate.EmailBodyText, tokenReplacements);
		var transformedBodyHtml = TokenTransformer.TransformTokens(messageTemplate.EmailBodyHtml, tokenReplacements);

		if (emailLog == null) emailLog = new EmailLog { CreatedDate = DateTime.Now };

		emailLog.MessageTemplateId = messageTemplate.Id;
		emailLog.From = from;
		emailLog.To = to;
		emailLog.AppointmentId = notification.AppointmentId;
		emailLog.Subject = transformedSubject;
		emailLog.BodyText = transformedBodyText;
		emailLog.BodyHtml = transformedBodyHtml;
		emailLog.ExceptionOccurred = false;
		emailLog.ExceptionMessage = null;

		try
		{
			await _schedulerQuery.PutEmailLogAsync(emailLog);
			if (!string.IsNullOrWhiteSpace(transformedBodyHtml))
			{
				if (transformedBodyHtml.Contains("{DigitalIntakeFormLink}"))
				{
					string intakeLink = null;
					if (emailLog.AppointmentId > 0)
					{
						var patientId = 0;
						var appt = await _schedulerQuery.GetAppointmentAsync(emailLog.AppointmentId, false);
						if (appt != null) patientId = appt.PatientId;
						if (patientId > 0)
						{
							var userId = await _providerQuery.GetUserIdFromProviderAsync(messageTemplate.ProviderId);
							var (success, link) =
								await _patientService.GetFormLinkAsync(PatientFormTypeEnum.Intake, patientId, userId);
							// Append a space, otherwise any text after the link will be concatenated and the link will fail.
							if (success) intakeLink = $"<a href=\"{link}\">{link}</a>";
							;
						}

						transformedBodyHtml = transformedBodyHtml.Replace("{DigitalIntakeFormLink}", intakeLink);
					}
				}

				transformedBodyHtml = transformedBodyHtml
					.Replace("<ConfirmationLink>",
						_GenerateResponseLink(baseUrl, officeCode, PatientNotificationResponse.Confirm, emailLog.Id,
							messageTemplate.ConfirmationLinkText))
					.Replace("<CancelLink>",
						_GenerateResponseLink(baseUrl, officeCode, PatientNotificationResponse.Cancel, emailLog.Id,
							messageTemplate.CancelLinkText))
					.Replace("<RescheduleLink>",
						_GenerateResponseLink(baseUrl, officeCode, PatientNotificationResponse.Reschedule, emailLog.Id,
							messageTemplate.CallToRescheduleLinkText));
			}

			await _schedulerQuery.PutEmailLogAsync(emailLog);

			await _mailgunEmailer.EmailAsync(from, to, transformedSubject, transformedBodyText,
				transformedBodyHtml, emailLog.Id, officeCode, _appSettings.Keys.ApiKey);
		}
		catch (Exception ex)
		{
			if (!isPreview)
			{
				// Add exception info to the log entry.
				emailLog.ExceptionOccurred = true;
				emailLog.ExceptionMessage = ex.GetBaseException().Message;
			}
		}
		finally
		{
			await _schedulerQuery.PutEmailLogAsync(emailLog);
		}

		if (!emailLog.ExceptionOccurred)
		{
			var emailTracking = new EmailTracking
			{
				EmailLogId = emailLog.Id,
				Status = "Sent",
				CreatedDate = DateTime.Now
			};
			await _schedulerQuery.PutEmailTrackingAsync(emailTracking);
		}

		if (!isPreview)
			await _schedulerQuery.AddNotificationStatusAsync(notification.AppointmentId,
				messageTemplate.TemplateType == MessageTemplateType.AppointmentConfirmation
					? NotificationStatus.EmailConfirmationSent
					: NotificationStatus.EmailReminderSent);
	}

	private async Task<Dictionary<int, string>> _SendPatientMessagesAsync(int appointmentId,
		Expression<Func<Appointment, bool>> predicate, MessageDeliveryMethod deliveryMethod,
		MessageTemplateType templateType)
	{
		var errors = new Dictionary<int, string>();
		try
		{
			var appointments = await _schedulerQuery.GetAppointmentsCustomQueryAsync(predicate);


			if (!appointments.Any())
			{
				errors[appointmentId] = "Appointment(s) not found. Please replicate and try again.";
				return errors;
			}

			var (webServer, officeCode, qbLocale) = await _practiceQuery.GetValueAsync(
				practice => new Tuple<string, string, string>(practice.TimsServer, practice.OfficeCode,
					practice.QbLocale));

			var businessRules = await _practiceQuery.GetBusinessRulesAsync();

			var nzOffice = qbLocale.ToLower() == "nz";
			var defaultLanguage = _GetLanguageEnum(CultureInfo.CurrentCulture.IetfLanguageTag);
			var cityStateZipFormat = defaultLanguage == LanguageEnum.EnglishAustralia ? "{0} {1} {2}" : "{0}, {1} {2}";


			var messagesGroupedByProvider = appointments.GroupBy(appt => appt.ProviderId).Select(group => new
			{
				ProviderId = group.Key,
				Messages = group.Select(appt => new PatientNotificationContainer<PatientNotification>
				{
					ToEmail = appt.Patient.Email,
					ToSms = appt.Patient.MobilePhone,
					ToVoice = string.IsNullOrWhiteSpace(appt.Patient.HomePhone)
						? appt.Patient.MobilePhone
						: appt.Patient.HomePhone,
					Model = new PatientNotification
					{
						AppointmentId = appt.Id,
						AppointmentEndsAt = appt.EndsAt,
						AppointmentStartsAt = appt.StartsAt,
						AppointmentType = appt.AppointmentType.Name,
						PatientFirstName = appt.Patient.FirstName,
						PatientLastName = appt.Patient.LastName,
						PatientId = appt.PatientId,
						ProviderFirstName = appt.Provider.FirstName,
						ProviderLastName = appt.Provider.LastName,
						AppointmentSite = appt.Site == null ? string.Empty : appt.Site.Name,
						AppointmentSiteAddress1 = appt.Site == null ? string.Empty : appt.Site.Address1,
						AppointmentSiteAddress2 = appt.Site == null ? string.Empty : appt.Site.Address2,
						AppointmentSiteCityStateZip = appt.Site == null
							? string.Empty
							: string.Format(cityStateZipFormat, appt.Site.City, appt.Site.State, appt.Site.Zip),
						AppointmentSitePhone = appt.Site == null ? string.Empty : appt.Site.Phone,
						Language = !appt.Patient.Language.HasValue || appt.Patient.Language.Value == LanguageEnum.None
							? defaultLanguage
							: appt.Patient.Language.Value,
						DateTimeFormat =
							new CultureInfo(nzOffice ? "en-NZ" : GetLanguageCodeFromEnum(defaultLanguage), false)
								.DateTimeFormat,
						ConfirmationLink = "<ConfirmationLink>",
						CancelLink = "<CancelLink>",
						CallForRescheduleLink = "<RescheduleLink>",
						DigitalPatientIntakeLink = "<DigitalPatientIntakeLink>"
					}
				})
			}).ToList();

			var messageSettings = await _practiceQuery.GetMessageSettingsAsync();

			foreach (var messageGroup in messagesGroupedByProvider)
			foreach (var message in messageGroup.Messages)
			{
				var messageTemplate = await _providerQuery.GetMessageTemplateAsync(messageGroup.ProviderId,
					templateType, message.Model.Language);

				if (message.Model.Language != messageTemplate.Language)
					message.Model.Language = messageTemplate.Language;
				//message.Model.DateTimeFormat = new CultureInfo(GetLanguageCodeFromEnum(message.Model.Language), false).DateTimeFormat;
				var notificationStatus =
					await _schedulerQuery.GetNotificationStatusAsync(message.Model.AppointmentId);

				string errorMessage;
				if (deliveryMethod == MessageDeliveryMethod.All || deliveryMethod == MessageDeliveryMethod.Email)
				{
					EmailLog emailLog = null;
					emailLog = await _schedulerQuery.GetEmailLogAsync(messageTemplate.Id, message.Model.AppointmentId);

					errorMessage = _CanSendEmailNotification(message.ToEmail, messageTemplate, notificationStatus,
						messageSettings, emailLog);
					if (string.IsNullOrEmpty(errorMessage))
						await _SendEmailNotificationAsync(message.ToEmail, messageTemplate, messageSettings,
							message.Model, webServer, officeCode, false, emailLog);
					else
						errors[message.Model.AppointmentId] = errorMessage;
				}

				if (deliveryMethod == MessageDeliveryMethod.All || deliveryMethod == MessageDeliveryMethod.Sms)
				{
					var smsLog = await _schedulerQuery.GetSmsLogAsync(messageTemplate.Id, message.Model.AppointmentId);
					errorMessage = _CanSendSmsNotification(message.ToSms, messageTemplate, notificationStatus,
						messageSettings, smsLog);
					if (string.IsNullOrEmpty(errorMessage))
						await _SendSmsNotificationAsync(message.ToSms, messageTemplate, messageSettings,
							message.Model, officeCode, false, smsLog, webServer, qbLocale,
							businessRules.UseAmazonMessaging);
					else
						errors[message.Model.AppointmentId] = errorMessage;
				}

				if (deliveryMethod == MessageDeliveryMethod.All || deliveryMethod == MessageDeliveryMethod.Voice)
				{
					var callLog =
						await _schedulerQuery.GetCallLogAsync(messageTemplate.Id, message.Model.AppointmentId);
					errorMessage = _CanSendVoiceNotification(message.ToVoice, messageTemplate, notificationStatus,
						messageSettings, callLog);
					if (string.IsNullOrEmpty(errorMessage))
						await _SendVoiceNotificationAsync(message.ToVoice, messageTemplate, messageSettings,
							message.Model, officeCode, false, callLog);
					else
						errors[message.Model.AppointmentId] = errorMessage;
				}
			}
		}
		catch (Exception ex)
		{
			errors[0] = ex.Message;
		}

		return errors;
	}

	private async Task _SendSmsNotificationAsync(string to, MessageTemplate messageTemplate,
		MessageSettings messageSettings, PatientNotification notification, string officeCode, bool isPreview,
		SmsLog smsLog, string serverUrl, string qbLocale, bool useAmazon)
	{
		var from = messageSettings.FromSmsNumber;
		var tokenReplacements = TokenTransformer.GetTokenReplacements(notification);
		var transformedSmsBody =
			TokenTransformer.TransformTokens(messageTemplate.GetSmsBody(messageTemplate.TemplateType),
				tokenReplacements);

		if (transformedSmsBody.Contains("{DigitalIntakeFormLink}"))
		{
			string intakeLink = null;
			var patientId = notification.PatientId;
			if (patientId > 0)
			{
				var userId = await _providerQuery.GetUserIdFromProviderAsync(messageTemplate.ProviderId);
				var (success, link) =
					await _patientService.GetFormLinkAsync(PatientFormTypeEnum.Intake, patientId, userId);
				// Append a space, otherwise any text after the link will be concatenated and the link will fail.
				if (success) intakeLink = link + " ";
			}

			transformedSmsBody = transformedSmsBody.Replace("{DigitalIntakeFormLink}", intakeLink);
		}

		if (smsLog == null) smsLog = new SmsLog();
		var createdDate = await _ConvertDateToCustomerTimeZoneAsync(officeCode, DateTime.Now);
		smsLog.MessageTemplateId = messageTemplate.Id;
		smsLog.From = from;
		smsLog.To = to.ToE164Format(qbLocale);
		smsLog.Body = transformedSmsBody;
		smsLog.AppointmentId = notification.AppointmentId;
		smsLog.PatientId = notification.PatientId;
		smsLog.CreatedDate = createdDate;
		smsLog.IsPreview = isPreview;
		smsLog.ExceptionOccurred = false;
		smsLog.ExceptionMessage = null;
		smsLog.CreatedUserId = _contextHelper.CurrentUser.Id;

		try
		{
			if (useAmazon)
				smsLog.Identifier = await _awsMessenger.SendSmsAsync(from, to, transformedSmsBody,
					officeCode, _appSettings.Keys.ApiKey, serverUrl, qbLocale);
			else
				smsLog.Identifier = await _twilioMessenger.SendSmsAsync(from, to, transformedSmsBody,
					officeCode, _appSettings.Keys.ApiKey, serverUrl, qbLocale);
		}
		catch (Exception ex)
		{
			// Add exception info to the log entry.
			smsLog.ExceptionOccurred = true;
			smsLog.ExceptionMessage = ex.GetBaseException().Message;
		}
		finally
		{
			await _schedulerQuery.PutSmsLogAsync(smsLog);
		}

		if (!isPreview)
			await _schedulerQuery.AddNotificationStatusAsync(notification.AppointmentId,
				messageTemplate.TemplateType == MessageTemplateType.AppointmentConfirmation
					? NotificationStatus.SmsConfirmationSent
					: NotificationStatus.SmsReminderSent);
		/*
		var chunkedBodySections = transformedSmsBody.SplitByLength(160);
		foreach(var chunkedBodySection in chunkedBodySections)
		{
		    var logEntry = new SmsLog()
		    {
		        MessageTemplateId = messageTemplate.Id,
		        From = from,
		        To = to.ToE164Format(),
		        Body = chunkedBodySection,
		        AppointmentId = notification.AppointmentId,
		        CreatedDate = DateTime.Now,
		        IsPreview = isPreview
		    };

		    try
		    {
		        logEntry.Identifier = await _twilioMessenger.SendSmsAsync(from, to, chunkedBodySection,
		            officeCode, _appSettings.Keys.ApiKey);
		    }
		    catch (Exception ex)
		    {
		        // Add exception info to the log entry.
		        logEntry.ExceptionOccurred = true;
		        logEntry.ExceptionMessage = ex.GetBaseException().Message;
		    }
		    finally
		    {
		        await _schedulerQuery.PutSmsLogAsync(logEntry);

		    }

		    if (logEntry.ExceptionOccurred)
		    {
		        throw new Exception(logEntry.ExceptionMessage);
		    }

		    if (!isPreview)
		    {
		        await _schedulerQuery.AddNotificationStatusAsync(notification.AppointmentId,
		            messageTemplate.TemplateType == MessageTemplateType.AppointmentConfirmation
		                ? NotificationStatus.SmsConfirmationSent
		                : NotificationStatus.SmsReminderSent);
		    }

		}

*/
	}

	private async Task _SendVoiceNotificationAsync(string to, MessageTemplate messageTemplate,
		MessageSettings messageSettings, PatientNotification notification, string officeCode, bool isPreview,
		VoiceCallLog callLog)
	{
		var from = messageSettings.FromPhoneNumber;
		var tokenReplacements = TokenTransformer.GetTokenReplacements(notification);
		var transformedCallScript =
			TokenTransformer.TransformTokens(messageTemplate.VoiceCallScript, tokenReplacements);

		if (callLog == null) callLog = new VoiceCallLog { CreatedDate = DateTime.Now };

		callLog.MessageTemplateId = messageTemplate.Id;
		callLog.From = from;
		callLog.To = to;
		callLog.CallScript = transformedCallScript;
		callLog.AppointmentId = notification.AppointmentId;
		callLog.IsPreview = isPreview;
		callLog.ExceptionOccurred = false;
		callLog.ExceptionMessage = null;

		try
		{
			callLog.Identifier = await _twilioMessenger.CallAsync(from, to, messageTemplate.TemplateType,
				officeCode, _appSettings.Keys.ApiKey);
		}
		catch (Exception ex)
		{
			// Add exception info to the log entry.
			callLog.ExceptionOccurred = true;
			callLog.ExceptionMessage = ex.GetBaseException().Message;
		}
		finally
		{
			await _schedulerQuery.PutCallLogAsync(callLog);
		}

		if (!isPreview)
			await _schedulerQuery.AddNotificationStatusAsync(notification.AppointmentId,
				messageTemplate.TemplateType == MessageTemplateType.AppointmentConfirmation
					? NotificationStatus.VoiceConfirmationSent
					: NotificationStatus.VoiceReminderSent);
	}

	public async Task<CallScriptTokens> GetCallScriptTokensAsync(VoiceCallLog callLog, string callbackBaseUrl,
		string officeCode, string apiKey)
	{
		var messageTemplate = await _providerQuery.GetMessageTemplateAsync(callLog.MessageTemplateId);

		var voice = "man";

		// Callscript may have voice encoded at beginning like <1>, <2> or <3>
		if (string.IsNullOrWhiteSpace(messageTemplate.Voice))
		{
			if (!string.IsNullOrWhiteSpace(callLog.CallScript) && callLog.CallScript.Length >= 3)
			{
				var encodedVoice = callLog.CallScript.Substring(0, 3);
				if (encodedVoice[0] == '<' && encodedVoice[2] == '>')
				{
					var voiceEnum = (TwilioVoiceEnum)int.Parse(encodedVoice[1].ToString());
					voice = voiceEnum.ToString().ToLower();
					callLog.CallScript = callLog.CallScript.Substring(3, callLog.CallScript.Length - 3);
				}
			}
		}
		else
		{
			voice = messageTemplate.Voice.Replace(" ", ".");
		}

		var serverUrl = await _practiceQuery.GetValueAsync(p => p.TimsServer);
		var queryParams = $"officeCode={officeCode}&key={apiKey}"; //VoiceStatusUpdate
		var uriBuilder = new UriBuilder(serverUrl);
		uriBuilder.AppendToPath("/api/WebHooks/CallResponse");
		uriBuilder.Query = queryParams;
		var responseUrl = new Uri(uriBuilder.ToString());

		return new CallScriptTokens
		{
			Voice = voice,
			Language = GetLanguageCodeFromEnum(messageTemplate.Language),
			Message = callLog.CallScript,
			ResponseUrl = responseUrl.ToString(),
			AllowCancel = messageTemplate.AllowVoiceCancel,
			AllowConfirm = messageTemplate.AllowVoiceConfirm,
			AllowReschedule = messageTemplate.AllowVoiceReschedule,
			VoicePromptText = messageTemplate.VoicePromptText
		};
	}


	public static string GetLanguageCodeFromEnum(Enum value)
	{
		if (value == null) return "en-US";
		var fi = value.GetType().GetField(value.ToString());
		var attributes = (LanguageCodeAttribute[])fi.GetCustomAttributes(
			typeof(LanguageCodeAttribute),
			false);

		if (attributes.Length > 0)
			return attributes[0].Code;
		// default to english-us
		return "en-US";
	}

	public static string ReplaceTokens(string callScriptTemplate, CallScriptTokens tokens)
	{
		return callScriptTemplate
			.Replace("{Voice}", tokens.Voice)
			.Replace("{Message}", tokens.Message)
			.Replace("{ResponseUrl}", tokens.ResponseUrl);
	}

	private static class TokenTransformer
	{
		public static IEnumerable<string> GetAvailableTokens<TModel>() where TModel : IPatientNotification
		{
			return typeof(TModel).GetProperties()
				.Where(p => p.Name.EndsWith("Placeholder"))
				.Select(p => p.Name.Replace("Placeholder", null))
				.ToList();
		}

		public static IDictionary<string, string> GetTokenReplacements<TModel>(TModel messageModel)
			where TModel : IPatientNotification
		{
			return typeof(TModel).GetProperties()
				.Where(p => p.Name.EndsWith("Placeholder"))
				.ToDictionary(p => p.Name.Replace("Placeholder", null),
					p => (string)p.GetValue(messageModel, null));
		}

		public static IEnumerable<string> GetTokens(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return Enumerable.Empty<string>();

			var matches = Regex.Matches(text, @"{(?<token>.*?)}").Cast<Match>();
			return matches.Select(m => m.Groups["token"].Value);
		}

		public static string TransformTokens(string text, IDictionary<string, string> tokenReplacements)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			var builder = new StringBuilder(text);

			foreach (var replacement in tokenReplacements)
			{
				// Tokens should be a la: {token}
				var token = string.Format("{{{0}}}", replacement.Key);
				var value = replacement.Value;

				builder.Replace(token, value);
			}

			return builder.ToString();
		}
	}
}