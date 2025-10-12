using System.Text.RegularExpressions;

namespace TIMS_X.Core.Utils
{
	public static class StringUtilities
	{
		public static string PadCenter(this string s, int width, char c)
		{
			if (s == null || width <= s.Length) return s;

			var padding = width - s.Length;
			return s.PadLeft(s.Length + padding / 2, c).PadRight(width, c);
		}

		/// <summary>
		///     Strips any nonnumeric data
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string StripNonNumeric(this string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return value;

			return Regex.Replace(value,
				"[^0-9]", // Select everything that is not in the range of 0-9
				string.Empty // Replace that with an empty string.
			);
		}
	}
}