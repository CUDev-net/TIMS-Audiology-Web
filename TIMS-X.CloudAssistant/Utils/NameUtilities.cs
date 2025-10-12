namespace TIMS_X.CloudAssistant.Utils
{
	public static class NameUtilities
	{
		public static bool PrivateModeEnabled { get; set; }

		#region NameUtilities Members

		/// <summary>
		///     Formats the provider name
		/// </summary>
		/// <param name="first"></param>
		/// <param name="middle"></param>
		/// <param name="last"></param>
		/// <returns></returns>
		public static string FormatPatientName(string first, string middle, string last)
		{
			if (PrivateModeEnabled)
			{
				var result = string.Empty;
				if (!string.IsNullOrWhiteSpace(first) && first.Length > 0) result = result + first[0] + ". ";
				if (!string.IsNullOrWhiteSpace(middle) && middle.Length > 0) result = result + middle[0] + ". ";
				if (!string.IsNullOrWhiteSpace(last) && last.Length > 0) result = result + last[0] + ".";

				return result.TrimEnd();
			}

			return last +
			       (string.IsNullOrEmpty(first) ? string.Empty : ", " + first) +
			       (string.IsNullOrEmpty(middle) ? string.Empty : " " + middle);
		}

		/// <summary>
		///     Formats the provider name
		/// </summary>
		/// <param name="first"></param>
		/// <param name="middle"></param>
		/// <param name="last"></param>
		/// <returns></returns>
		public static string FormatProviderName(string first, string middle, string last)
		{
			if (string.IsNullOrEmpty(first) || string.IsNullOrEmpty(last)) return "PROVIDER NOF";
			if (string.IsNullOrEmpty(middle))
				return first + " " + last;
			return first + " " + middle + " " + last;
		}

		#endregion NameUtilities Members
	}
}