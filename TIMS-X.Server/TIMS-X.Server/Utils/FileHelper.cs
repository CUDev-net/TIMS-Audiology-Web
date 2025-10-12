namespace TIMS_X.Server.Utils;

public static class FileHelper
{
	public static string SanitizeFilename(string proposedFileName)
	{
		return proposedFileName.Replace("<", "_")
			.Replace(">", "_")
			.Replace(":", "_")
			.Replace("\"", "_")
			.Replace("/", "_")
			.Replace("\\", "_")
			.Replace("|", "_")
			.Replace("?", "_")
			.Replace("*", "_");
	}
}