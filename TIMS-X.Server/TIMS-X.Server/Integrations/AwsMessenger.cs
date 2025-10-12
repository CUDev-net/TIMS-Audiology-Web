using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.PinpointSMSVoiceV2;
using Amazon.PinpointSMSVoiceV2.Model;
using Microsoft.Extensions.Configuration;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Extensions;
using TIMS_X.Server.Config;
using TIMS_X.Server.Extensions;
using TIMS_X.Server.Queries;

namespace TIMS_X.Server.Integrations;

// Amazon Aws Sms/Voice messaging implementation.
// Based on combination of TwilioMessenger and the code in the guide found here:
// https://docs.aws.amazon.com/sms-voice/latest/userguide/send-sms-voice-message.html#sms-voice-v2-messages-sms
// and referencing docs found here: https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/PinpointSMSVoiceV2/TPinpointSMSVoiceV2Client.html

public class AwsMessenger
{
	private readonly AppSettings _appSettings;
	private readonly MessageSettings _messageSettings;
	private readonly PracticeQuery _practiceQuery;

	private readonly AmazonPinpointSMSVoiceV2Client _awsClient;

	public AwsMessenger(PracticeQuery practiceQuery, IConfiguration configuration)
	{
		_practiceQuery = practiceQuery;
		_appSettings = configuration.Get<AppSettings>();
		_messageSettings = _practiceQuery.GetMessageSettings();
		if (_messageSettings != null &&
		    !string.IsNullOrWhiteSpace(_messageSettings.AccountSid) &&
		    !string.IsNullOrWhiteSpace(_messageSettings.AuthorizationToken))
			_awsClient = new AmazonPinpointSMSVoiceV2Client(
				_messageSettings.AccountSid,
				_messageSettings.AuthorizationToken,
				RegionEndpoint.USEast2);
	}

	public bool IsEnabled => true;

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
		//CallResource call;
		//try
		//{
		//    ServicePointManager.SecurityProtocol = 
		//         SecurityProtocolType.Tls  |
		//        (SecurityProtocolType)768  | //TLS 1.1
		//        (SecurityProtocolType)3072 ; //TLS 1.2

		//    call = CallResource.Create(
		//        to: new PhoneNumber(formattedTo),
		//        from: new PhoneNumber(formattedFrom),
		//        url: callScriptUrl,
		//        client: _twilioClient,
		//        statusCallback: statusUrl,
		//        machineDetection: "DetectMessageEnd");
		//}
		//catch (ApiException e)
		//{
		//    string error = "Error Initiating call: " + e.Message;
		//    throw new Exception(error);
		//}

		//return call.Sid;
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


		var dryRun = false;
		var configurationSet = "arn:aws:sms-voice:us-east-2:329599656014:configuration-set/tims_pool1";
		var envConfigurationSet = Environment.GetEnvironmentVariable("AWS_CONFIGURATION_SET");
		if (!string.IsNullOrEmpty(envConfigurationSet)) configurationSet = envConfigurationSet.Trim();

		var maxPrice = "2.00";
		var contextKeys = new Dictionary<string, string>();

		// messageType can be either "TRANSACTIONAL" or "PROMOTIONAL"
		var messageType = "TRANSACTIONAL";

		// amount of time in seconds that aws should attempt to deliver the message. Max  259200 seconds (72 hours)
		var ttl = 120;

		var response = await _awsClient.SendTextMessageAsync(
			new SendTextMessageRequest
			{
				ConfigurationSetName = configurationSet,
				Context = contextKeys,
				OriginationIdentity = formattedFrom,
				DestinationPhoneNumber = formattedTo,
				DryRun = dryRun,
				MaxPrice = maxPrice,
				MessageType = messageType,
				TimeToLive = ttl,
				MessageBody = body
			});

		return response.MessageId;
	}
}