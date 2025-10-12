using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TIMS_X.Core;
using TIMS_X.Server.Config;
using TIMS_X.Server.Integrations;
using TIMS_X.Server.Queries;

namespace TIMS_X.Server.Services;

public class UserSecurityService
{
	private readonly AppSettings _appSettings;
	private readonly ContextHelper _contextHelper;
	private readonly MailgunEmailer _mailgunEmailer;
	private readonly PracticeQuery _practiceQuery;

	private readonly TwilioMessenger _twilioMessenger;

	public UserSecurityService(IOptions<AppSettings> appSettings, PracticeQuery practiceQuery,
		TwilioMessenger twilioMessenger, MailgunEmailer mailgunEmailer, ContextHelper contextHelper)
	{
		_appSettings = appSettings.Value;
		_practiceQuery = practiceQuery;
		_twilioMessenger = twilioMessenger;
		_mailgunEmailer = mailgunEmailer;
		_contextHelper = contextHelper;
	}

	public async Task<bool> SendOneTimePasscodeAsync(string passcode)
	{
		if (_contextHelper.CurrentUser == null || string.IsNullOrEmpty(passcode) ||
		    (string.IsNullOrEmpty(_contextHelper.CurrentUser.MobilePhone) &&
		     string.IsNullOrEmpty(_contextHelper.CurrentUser.Email)))
			return false;

		var messageSettings = await _practiceQuery.GetMessageSettingsAsync();
		var (officeCode, serverUrl, qbLocale) = await _practiceQuery.GetValueAsync(p =>
			new Tuple<string, string, string>(p.OfficeCode, p.TimsServer, p.QbLocale));

		var body = $"Security code for TIMS: {passcode}";


		if (messageSettings.IsSmsEnabled && !string.IsNullOrEmpty(_contextHelper.CurrentUser.MobilePhone))
			await _twilioMessenger.SendSmsAsync(messageSettings.FromSmsNumber, _contextHelper.CurrentUser.MobilePhone,
				body, officeCode, _appSettings.Keys.ApiKey, serverUrl, qbLocale, false);

		if (messageSettings.IsEmailEnabled && !string.IsNullOrEmpty(_contextHelper.CurrentUser.Email))
			await _mailgunEmailer.EmailAsync(messageSettings.FromEmailAddress, _contextHelper.CurrentUser.Email,
				"Security code for TIMS", body, null, 0, officeCode, _appSettings.Keys.ApiKey);

		return true;
	}
}