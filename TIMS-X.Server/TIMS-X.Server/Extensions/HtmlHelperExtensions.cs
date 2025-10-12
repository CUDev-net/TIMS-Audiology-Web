using System;

namespace TIMS_X.Server.Extensions;

public static class HtmlHelperExtensions
{
	public static string FirstCharToUpper(this string input)
	{
		if (string.IsNullOrEmpty(input)) return string.Empty;

		return string.Create(input.Length, input, static (chars, str) =>
		{
			chars[0] = char.ToUpperInvariant(str[0]);
			str.AsSpan(1).CopyTo(chars[1..]);
		});
	}

	public static bool IsDebug()
	{
#if DEBUG
		return true;
#elif TEST
            return true;
#else
            return false;
#endif
	}
}