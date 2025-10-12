using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TIMS_X.Core.Extensions
{
    public static class StringExtensions
    {
        public static List<string> SplitByLength(this string s, int maxChars, bool honorWordBoundaries = true)
        {
            var chunks = new List<string>();

            if (s == null || s.Length <= maxChars)
            {
                chunks.Add(s);
            }
            else
            {
                var stringLength = s.Length;
                var index = 0;

                if (honorWordBoundaries)
                {
                    while (index < stringLength)
                    {
                        // Find the last index of a space within maxChars.
                        var count = Math.Min(maxChars, stringLength - index);
                        var startIndex = index + count;
                        int length;

                        if (startIndex == stringLength)
                        {
                            length = count;
                        }
                        else
                        {
                            // If there isn't a space in the section:
                            // - perform a hard break if within the string,
                            // - take the rest of the string if at the end of the string;
                            // otherwise, take up to and including the space.
                            var lastIndexOfSpace = s.LastIndexOf(" ", startIndex, count, StringComparison.OrdinalIgnoreCase);
                            if (lastIndexOfSpace == -1)
                            {
                                length = count;
                            }
                            else
                            {
                                length = lastIndexOfSpace + 1 - index;
                            }
                        }

                        chunks.Add(s.Substring(index, length));
                        index += length;
                    }
                }
                else // not honoring word boundaries
                {
                    while (index < stringLength)
                    {
                        // Split every maxChars.
                        var length = Math.Min(maxChars, stringLength - index);
                        chunks.Add(s.Substring(index, length));
                        index += length;
                    }
                }
            }

            return chunks;
        }

        public static string StripNonNumeric(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            return Regex.Replace(value,
                "[^0-9]", // Select everything that is not in the range of 0-9
                string.Empty        // Replace that with an empty string.
                );
        }
    }
}
