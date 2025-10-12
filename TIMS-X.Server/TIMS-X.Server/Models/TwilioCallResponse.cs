namespace TIMS_X.Server.Models;

public class TwilioCallResponse
{
	/// <summary>
	///     The id of the Call as provided by Twilio.
	/// </summary>
	public string CallSid { get; set; }

	/// <summary>
	///     The Call status.
	/// </summary>
	public string CallStatus { get; set; }

	/// <summary>
	///     The digits that the user pressed, including the termination symbol (*, #, etc...).
	/// </summary>
	public string Digits { get; set; }
}