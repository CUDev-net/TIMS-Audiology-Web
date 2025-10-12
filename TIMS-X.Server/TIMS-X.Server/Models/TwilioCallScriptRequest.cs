namespace TIMS_X.Server.Models;

public class TwilioCallScriptRequest
{
	public string AnsweredBy { get; set; }

	/// <summary>
	///     The id of the Call as provided by Twilio.
	/// </summary>
	public string CallSid { get; set; }
}