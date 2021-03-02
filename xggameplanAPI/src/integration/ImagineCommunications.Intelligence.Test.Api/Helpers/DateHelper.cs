using System;
using System.Collections.Generic;

namespace ImagineCommunications.Intelligence.Test.Api.Helpers
{
    public static class DateHelper
    {
        private static readonly IDictionary<string, Func<string, DateTimeKind, DateTime>> SupportedDateFormats =
            new Dictionary<string, Func<string, DateTimeKind, DateTime>>()
            {
                { "yyyyMMdd", (dateStr, kind) => GetDate(dateStr, (0, 4), (4, 2), (6, 2), kind) },
                { "yyyy-MM-dd", (dateStr, kind) => GetDate(dateStr, (0, 4), (5, 2), (8, 2), kind) },
                { "dd-MM-yyyy", (dateStr, kind) => GetDate(dateStr, (6, 4), (3, 2), (0, 2), kind) }
            };

        public static DateTime GetDate(string value, string format, DateTimeKind kind = DateTimeKind.Local)
        {
            if (SupportedDateFormats.TryGetValue(format, out var dateParser))
            {
                return dateParser(value, kind);
            }
            throw new NotImplementedException();
        }


        public static DateTime GetDate(string dateStr, (int, int) year, (int, int) month, (int, int) day, DateTimeKind kind)
        {
            return new DateTime(
                Convert.ToInt32(dateStr.Substring(year.Item1, year.Item2)),
                Convert.ToInt32(dateStr.Substring(month.Item1, month.Item2)),
                Convert.ToInt32(dateStr.Substring(day.Item1, day.Item2)),
                0, 0, 0, kind);
        }
    }
}
