using System;
using System.Globalization;

namespace xggameplan.core.ReportGenerators
{
    public static class ReportFormatter
    {
        private const string DateTimeFormat = "dd/MM/yyyy HH:mm:ss";
        private const string DateFormat = "dd/MM/yyyy";
        private const string TimeFormat = @"hh\:mm\:ss";

        public static string ConvertToDateTime(object dateTime) =>
            ((DateTime)dateTime).ToUniversalTime().ToString(DateTimeFormat);

        public static string ConvertToShortDate(object dateTime) =>
            dateTime != null ? ((DateTime)dateTime).ToUniversalTime().ToString(DateFormat) : string.Empty;

        public static string ConvertToTime(object timeSpan) =>
            ((TimeSpan)timeSpan).ToString(TimeFormat);

        public static string DecimalRoundingFormatter(object rawValue) =>
            rawValue is decimal value
                ? Math.Round(value, 2).ToString(CultureInfo.InvariantCulture)
                : string.Empty;
    }
}
