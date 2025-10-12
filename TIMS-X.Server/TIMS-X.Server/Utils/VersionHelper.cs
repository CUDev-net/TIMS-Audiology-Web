using System.Reflection;

namespace TIMS_X.Server.Utils;

public static class VersionHelper
{
	public static string ServerVersion => "v" + Assembly.GetExecutingAssembly().GetName().Version;
}