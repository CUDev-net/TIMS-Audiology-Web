namespace TIMS_X.Server.Models;

public class CallScriptTokens
{
	public bool AllowCancel { get; set; }
	public bool AllowConfirm { get; set; }
	public bool AllowReschedule { get; set; }
	public string Language { get; set; }
	public string Message { get; set; }
	public string ResponseUrl { get; set; }
	public string Voice { get; set; }
	public string VoicePromptText { get; set; }
}