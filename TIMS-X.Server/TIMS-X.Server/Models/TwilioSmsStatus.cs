namespace TIMS_X.Server.Models;

public class TwilioSmsStatus
{
	/// <summary>
	///     The id of the SMS as provided by Twilio.
	/// </summary>
	public string SmsSid { get; set; }

	/// <summary>
	///     The SMS status.
	/// </summary>
	public string SmsStatus { get; set; }
}