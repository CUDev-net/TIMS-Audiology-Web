using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Extensions;
using TIMS_X.Server.Extensions;
using TIMS_X.Server.Queries;
using Twilio;
using Twilio.Clients;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Api.V2010.Account.Usage.Record;
using Twilio.Types;

namespace TIMS_X.Server.Integrations;

public class TwilioMessenger
{
	private readonly MessageSettings _messageSettings;
	private readonly PracticeQuery _practiceQuery;
	private readonly TwilioRestClient _twilioClient;

	public TwilioMessenger(PracticeQuery practiceQuery)
	{
		_practiceQuery = practiceQuery;
		_messageSettings = _practiceQuery.GetMessageSettings();

		if (_messageSettings != null &&
		    !string.IsNullOrWhiteSpace(_messageSettings.AccountSid) &&
		    !string.IsNullOrWhiteSpace(_messageSettings.AuthorizationToken))
			_twilioClient = new TwilioRestClient(_messageSettings.AccountSid,
				_messageSettings.AuthorizationToken);
	}


	public bool IsEnabled => _twilioClient != null;

	public async Task<string> CallAsync(string from, string to, MessageTemplateType templateType, string officeCode,
		string apiKey)
	{
		if (!IsEnabled) throw new InvalidOperationException("Twilio Messenger not enabled");
		// Check constraints
		if (from == null) throw new ArgumentNullException("from");
		if (to == null) throw new ArgumentNullException("to");
		if (templateType == MessageTemplateType.Unknown) throw new ArgumentOutOfRangeException("templateType");

		var qbLocale = await _practiceQuery.GetValueAsync(p => p.QbLocale);

		var businessRules = await _practiceQuery.GetBusinessRulesAsync();
		var fromQbLocale = "us";
		if (!businessRules.SendsNotificationsFromUSNumber) fromQbLocale = qbLocale;

		// Format the phone numbers in E.164 format
		var formattedFrom = from.ToE164Format(fromQbLocale);
		var formattedTo = to.ToE164Format(qbLocale);
		var serverUrl = await _practiceQuery.GetValueAsync(p => p.TimsServer);
		var type = (int)templateType;
		var queryParams = $"templateType={type}&officeCode={officeCode}&key={apiKey}"; //VoiceStatusUpdate
		var uriBuilder = new UriBuilder(serverUrl);
		uriBuilder.AppendToPath("/api/WebHooks/CallScript");
		uriBuilder.Query = queryParams;
		var callScriptUrl = new Uri(uriBuilder.ToString());

		uriBuilder = new UriBuilder(serverUrl);
		uriBuilder.AppendToPath("/api/WebHooks/CallStatus");
		uriBuilder.Query = queryParams;
		var statusUrl = new Uri(uriBuilder.ToString());
		CallResource call;
		try
		{
			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Tls |
				(SecurityProtocolType)768 | //TLS 1.1
				(SecurityProtocolType)3072; //TLS 1.2

			call = CallResource.Create(
				new PhoneNumber(formattedTo),
				new PhoneNumber(formattedFrom),
				url: callScriptUrl,
				client: _twilioClient,
				statusCallback: statusUrl,
				machineDetection: "DetectMessageEnd");
		}
		catch (ApiException e)
		{
			var error = "Error Initiating call: " + e.Message;
			throw new Exception(error);
		}

		return call.Sid;
	}


	public List<string> GetMonthlyUsage()
	{
		if (_messageSettings != null &&
		    !string.IsNullOrWhiteSpace(_messageSettings.AccountSid) &&
		    !string.IsNullOrWhiteSpace(_messageSettings.AuthorizationToken))
		{
			TwilioClient.Init(_messageSettings.AccountSid,
				_messageSettings.AuthorizationToken);

			var records = LastMonthResource.Read();

			var result = new List<string>();

			foreach (var record in records.Where(r => r.Price.HasValue && r.Price.Value > 0.0m))
				result.Add(
					$"{record.Description}, {record.Count}, {record.StartDate}, {record.EndDate}, {record.Price}");

			return result;
		}

		return null;
	}

	public async Task<string> SendSmsAsync(string from, string to, string body, string officeCode, string apiKey,
		string serverUrl, string qbLocale, bool setStatusCallbackUrl = true)
	{
		// Check constraints
		if (from == null) throw new ArgumentNullException("from");
		if (to == null) throw new ArgumentNullException("to");
		if (body == null) throw new ArgumentNullException("message");

		var businessRules = await _practiceQuery.GetBusinessRulesAsync();
		var fromQbLocale = "us";
		if (!businessRules.SendsNotificationsFromUSNumber) fromQbLocale = qbLocale;


		// Format the phone numbers in E.164 format
		var formattedFrom = from.ToE164Format(fromQbLocale);
		var formattedTo = to.ToE164Format(qbLocale);


		var queryParams = $"officeCode={officeCode}&key={apiKey}"; //VoiceStatusUpdate
		var uriBuilder = new UriBuilder(serverUrl);
		uriBuilder.AppendToPath("/api/WebHooks/SmsStatus");
		uriBuilder.Query = queryParams;
		var statusCallbackUrl = new Uri(uriBuilder.ToString());

		// Send message via Twilio REST API
		MessageResource message;
		try
		{
			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Tls |
				(SecurityProtocolType)768 | //TLS 1.1
				(SecurityProtocolType)3072; //TLS 1.2


			message = MessageResource.Create(
				from: new PhoneNumber(formattedFrom),
				to: new PhoneNumber(formattedTo),
				body: body,
				client: _twilioClient,
				statusCallback: setStatusCallbackUrl ? statusCallbackUrl : null);
		}
		catch (ApiException e)
		{
			var error = "Error Sending Sms: " + e.Message;
			throw new Exception(error);
		}

		return message.Sid;
	}
}