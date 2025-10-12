namespace TIMS_X.Server.Config;

public class AppSettings
{
	public ConnectionStrings ConnectionStrings { get; set; }
	public Keys Keys { get; set; }

	public string LogFile { get; set; }
}

public class ConnectionStrings
{
	public string TIMSInternal { get; set; }
}

public class Keys
{
	public string ApiKey { get; set; }
	public string AwsAccessKey { get; set; }
	public string AwsAccessKeySecret { get; set; }
	public string DbPassword { get; set; }
	public string DbUsername { get; set; }
	public string DefaultPassword { get; set; }
	public string HelpCookie { get; set; }
	public string HelpPassword6_8 { get; set; }
	public string ImagingKey { get; set; }
	public string JwtSecret { get; set; }
	public string MailgunApiKey { get; set; }
	public string SupportPassword { get; set; }
	public string TimsToken { get; set; }
	public string TwilioAccountSid { get; set; }
	public string TwilioAuthToken { get; set; }
	public string TwilioSmsStatusCallbackUrl { get; set; }
	public string TwilioVoiceCallScriptCallbackUrl { get; set; }
	public string TwilioVoiceStatusCallbackUrl { get; set; }
	public string ZoomTokenAuthUrl { get; set; }
	public string ZoomUserAuthUrl { get; set; }
}