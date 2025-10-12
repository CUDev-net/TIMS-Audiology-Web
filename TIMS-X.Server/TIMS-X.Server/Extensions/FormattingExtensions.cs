using System;
using System.Text.RegularExpressions;

namespace TIMS_X.Server.Extensions;

public static class FormattingExtensions
{
	#region FormattingExtensions Members

	public static string ToE164Format(this string phoneNumber, string qbLocale)
	{
		var replacementPatterns = new[]
		{
			@"^\s*\+1", // Any leading +1
			@"\D" // Any non-digit
		};

		var scrubbedPhoneNumber = Regex.Replace(phoneNumber, string.Join("|", replacementPatterns), "");

		if (qbLocale != null && qbLocale.ToLower() == "nz") return "+64" + scrubbedPhoneNumber;

		if (qbLocale != null && qbLocale.ToLower() == "uk") return "+44" + scrubbedPhoneNumber;

		if (qbLocale != null && qbLocale.ToLower() == "au")
		{
			return "+61" + scrubbedPhoneNumber;
		}

		// Ensure we have a 10-digit phone number
		if (scrubbedPhoneNumber.Length != 10)
			throw new ArgumentException($"The phone number \"{phoneNumber}\" is not a 10-digit phone number.",
				"phoneNumber");

		return "+1" + scrubbedPhoneNumber;
	}

	public static string Digits(this string phoneNumber)
	{
		var result = string.Empty;
		if (phoneNumber != null)
			foreach (var character in phoneNumber)
				if (char.IsDigit(character))
					result += character;

		if (result.Length > 10) result = result.Substring(1, 10);

		return result;
	}

	#endregion FormattingExtensions Members
}