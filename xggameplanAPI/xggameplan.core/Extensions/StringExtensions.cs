using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using xggameplan.common.Extensions;

namespace xggameplan.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex _doubleSpaceRegEx = new Regex(@" {2,}", RegexOptions.Compiled);

        [Obsolete("Use the method in the class " + nameof(EnumerableOfStringExtensions))]
        public static List<string> Trim(this IEnumerable<string> values) => EnumerableOfStringExtensions.Trim(values);

        public static string ReduceExcessSpace(this string content)
        {
            if (string.IsNullOrWhiteSpace(content)) { return content; }

            return _doubleSpaceRegEx.Replace(content.Trim(), " ");
        }

        public static TimeSpan? ToTime(this string time)
        {
            TimeSpan result;
            if (!string.IsNullOrWhiteSpace(time) && TimeSpan.TryParse(time.Trim(), CultureInfo.InvariantCulture, out result)) { return result; }
            return null;
        }

        public static bool StartsWithInvariantCultureIgnoreCase(this string self, string other) =>
            self?.StartsWith(other, StringComparison.InvariantCultureIgnoreCase) ?? false;

        public static bool EqualInvariantCultureIgnoreCase(this string self, string other) =>
            self?.Equals(other, StringComparison.InvariantCultureIgnoreCase) ?? false;
    }
}
