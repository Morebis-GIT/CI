using System;
using System.Collections.Generic;
using System.Linq;

namespace xggameplan.common.Extensions
{
    public static class EnumerableOfStringExtensions
    {
        public static List<string> Trim(this IEnumerable<string> values)
        {
            if (values is null || !values.Any() || values.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentNullException(nameof(values));
            }

            return values.Select(s => s.Trim()).ToList();
        }
    }
}
