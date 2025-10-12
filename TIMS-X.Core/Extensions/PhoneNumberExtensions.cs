using System.Text;

namespace TIMS_X.Core.Extensions
{
    public static class PhoneNumberExtensionMethods
    {
        public static string FormatFax(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var c in source)
                if (char.IsDigit(c))
                    sb.Append(c);
            if (sb.Length == 10) return "1" + sb;
            return sb.ToString();
        }

        public static string FormatPhone(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            var formatted = string.Empty;

            if (source.Length > 12)
            {
                var sb = new StringBuilder();
                foreach (var c in source)
                {
                    // strip out all non-numerics
                    if (char.IsDigit(c)) sb.Append(c);
                    var digits = sb.ToString();
                    if (digits.Length == 10)
                        formatted = string.Format("{0} {1}-{2}", digits.Substring(0, 3), digits.Substring(3, 3),
                            digits.Substring(6, 4));
                    else if (digits.Length == 7)
                        formatted = string.Format("{0}-{1}", digits.Substring(0, 3), digits.Substring(3, 4));
                    else if (digits.Length > 12)
                        formatted = digits.Substring(0, 12);
                    else
                        formatted = digits;
                }
            }
            else
            {
                formatted = source;
            }

            return formatted;
        }
    }
}