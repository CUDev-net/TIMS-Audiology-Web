using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Models;
using TIMS_X.Server.Queries;

namespace TIMS_X.Server.Integrations;

public class MailgunEmailer
{
	private readonly PracticeQuery _practiceQuery;
	private readonly SchedulerQuery _schedulerQuery;
	private readonly SmtpClient _smtpClient;
	private readonly string X_MAILGUN_VARIABLES = "X-Mailgun-Variables";

	public MailgunEmailer(PracticeQuery practiceQuery, SchedulerQuery schedulerQuery)
	{
		_practiceQuery = practiceQuery;
		_schedulerQuery = schedulerQuery;
		_smtpClient = new SmtpClient("smtp.mailgun.org", 587);
		_smtpClient.EnableSsl = true;
		_smtpClient.Credentials =
			new NetworkCredential("postmaster@timsaudiology.com", "3e18c73599fbfcd1db9ae247780924b5");
	}

	public bool IsEnabled => true;

	public async Task EmailAsync(PatientMessageModel model, string officeCode, string apiKey,
		MessageSettings messageSettings)
	{
		if (!IsEnabled) throw new InvalidOperationException("Mailgun Emailer not enabled");

		// Check constraints
		if (messageSettings.FromEmailAddress == null) throw new ArgumentNullException("from");
		if (model.EmailAddress == null) throw new ArgumentNullException("to");

		var message = new MailMessage(messageSettings.FromEmailAddress, model.EmailAddress)
		{
			Subject = model.EmailSubject
		};

		var modelMessage = model.Message;

		var emailDisclaimer = await _practiceQuery.GetValueAsync(
			practice => practice.EmailDisclaimer);


		if (!string.IsNullOrEmpty(emailDisclaimer))
		{
			modelMessage = emailDisclaimer;
			if (!string.IsNullOrEmpty(model.Message)) modelMessage = modelMessage + model.Message;
		}

		if (!string.IsNullOrWhiteSpace(modelMessage))
		{
			var textView = AlternateView.CreateAlternateViewFromString(modelMessage, new ContentType("text/plain"));
			message.AlternateViews.Add(textView);
		}

		var emailLog = new EmailLog { CreatedDate = DateTime.Now };
		emailLog.MessageTemplateId = 0;
		emailLog.From = messageSettings.FromEmailAddress;
		emailLog.To = model.EmailAddress;
		emailLog.PatientId = model.PatientId;
		emailLog.AppointmentId = 0;
		emailLog.Subject = model.EmailSubject;
		emailLog.BodyText = modelMessage;
		emailLog.ExceptionOccurred = false;
		emailLog.ExceptionMessage = null;

		await _schedulerQuery.PutEmailLogAsync(emailLog);

		var variables = new Dictionary<string, object>
		{
			{ "EmailLogId", emailLog.Id },
			{ "OfficeCode", officeCode },
			{ "ApiKey", apiKey }
		};

		message.Headers.Add(X_MAILGUN_VARIABLES, JsonConvert.SerializeObject(variables));

		try
		{
			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Tls |
				(SecurityProtocolType)768 | //TLS 1.1
				(SecurityProtocolType)3072; //TLS 1.2

			_smtpClient.Send(message);
		}
		catch (Exception e)
		{
			var error = "Error Sending Email: " + e.Message;
			throw new Exception(error);
		}
	}

	public async Task EmailAsync(string from, string to, string subject, string bodyText, string bodyHtml, int logId,
		string officeCode, string apiKey)
	{
		if (!IsEnabled) throw new InvalidOperationException("Mailgun Emailer not enabled");
		// Check constraints
		if (from == null) throw new ArgumentNullException("from");
		if (to == null) throw new ArgumentNullException("to");

		var message = new MailMessage(from, to)
		{
			Subject = subject
		};

		var emailDisclaimer = await _practiceQuery.GetValueAsync(
			practice => practice.EmailDisclaimer);

		if (!string.IsNullOrWhiteSpace(bodyText))
		{
			if (!string.IsNullOrEmpty(emailDisclaimer)) bodyText = emailDisclaimer + bodyText;
			var textView = AlternateView.CreateAlternateViewFromString(bodyText, new ContentType("text/plain"));
			message.AlternateViews.Add(textView);
		}

		if (!string.IsNullOrWhiteSpace(bodyHtml))
		{
			if (!string.IsNullOrEmpty(emailDisclaimer)) bodyHtml = emailDisclaimer + bodyHtml;

			var body = bodyHtml
				.Replace(Environment.NewLine, "<br />")
				.Replace("\t", "&emsp;");

			var htmlView = AlternateView.CreateAlternateViewFromString(body, new ContentType("text/html"));
			message.AlternateViews.Add(htmlView);
		}

		var variables = new Dictionary<string, object>
		{
			{ "EmailLogId", logId },
			{ "OfficeCode", officeCode },
			{ "ApiKey", apiKey }
		};

		message.Headers.Add(X_MAILGUN_VARIABLES, JsonConvert.SerializeObject(variables));

		try
		{
			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Tls |
				(SecurityProtocolType)768 | //TLS 1.1
				(SecurityProtocolType)3072; //TLS 1.2

			_smtpClient.Send(message);
		}
		catch (Exception e)
		{
			var error = "Error Sending Email: " + e.Message;
			throw new Exception(error);
		}
	}
}