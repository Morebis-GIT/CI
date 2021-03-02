using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Threading;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using NodaTime;

namespace xggameplan.common.Helpers
{
    public static class LogAsString
    {
        public static string Ellipsis => "\u2026";

        /// <summary>
        /// Value to place in a log entry to enable grouping of similar entries.
        /// </summary>
        [Pure]
        public static string LogEntryDiscriminator =>
            $"[TID: {Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture)}] ";

        /// <summary>
        /// Convert the <see cref="DateTime"/> value to ISO 8601 format.
        /// </summary>
        /// <param name="value">A <see cref="DateTime"/> value to convert for logging.</param>
        /// <returns>A string representation of the <see cref="DateTime"/> value.</returns>
        [Pure]
        public static string Log(DateTime value) => value
            .ToUniversalTime()
            .ToString("O", CultureInfo.InvariantCulture);

        /// <summary>
        /// Convert the <see cref="ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges.DateTimeRange"/> value to ISO 8601 format.
        /// </summary>
        /// <param name="value">A <see cref="ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges.DateTimeRange"/> value to convert for logging.</param>
        /// <returns>A string representation of the <see cref="ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges.DateTimeRange"/> value.</returns>
        [Pure]
        public static string Log(DateTimeRange value) => value.ToString();

        /// <summary>Logs the specified duration as seconds.</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [Pure]
        public static string Log(Duration value) => value
            .ToTimeSpan()
            .TotalSeconds
            .ToString(CultureInfo.InvariantCulture);

        [Pure]
        public static string Log(Guid value) => value.ToString();

        [Pure]
        public static string Log(long value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
