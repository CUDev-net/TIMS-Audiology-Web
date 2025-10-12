namespace TIMS_X.Server.Models;

public class TwilioSmsResponse
{
	/// <summary>
	///     The 34 character id of the Account this message is associated with.
	/// </summary>
	public string AccountSid { get; set; }

	/// <summary>
	///     The text body of the SMS message. Up to 160 characters long.
	/// </summary>
	public string Body { get; set; }

	/// <summary>
	///     The phone number that sent this message.
	/// </summary>
	public string From { get; set; }

	public string MediaContentType0 { get; set; }

	public string MediaContentType1 { get; set; }

	public string MediaContentType2 { get; set; }

	public string MediaContentType3 { get; set; }
	public string MediaUrl0 { get; set; }
	public string MediaUrl1 { get; set; }
	public string MediaUrl2 { get; set; }
	public string MediaUrl3 { get; set; }

	/// <summary>
	///     The number of attached media
	/// </summary>
	public int NumMedia { get; set; }

	/// <summary>
	///     A 34 character unique identifier for the message.
	///     May be used to later retrieve this message from the REST API.
	/// </summary>
	public string SmsSid { get; set; }

	/// <summary>
	///     The sms status.
	/// </summary>
	public string SmsStatus { get; set; }

	/// <summary>
	///     The phone number of the recipient.
	/// </summary>
	public string To { get; set; }
}