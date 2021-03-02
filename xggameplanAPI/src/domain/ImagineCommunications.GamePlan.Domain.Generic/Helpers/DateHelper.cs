using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;

namespace ImagineCommunications.GamePlan.Domain.Generic.Helpers
{
    public static class DateHelper
    {
        public static DateTime GetDate(string value, string format, DateTimeKind kind = DateTimeKind.Local)
        {
            switch (format)
            {
                case "yyyyMMdd": return new DateTime(Convert.ToInt32(value.Substring(0, 4)), Convert.ToInt32(value.Substring(4, 2)), Convert.ToInt32(value.Substring(6, 2)), 0, 0, 0, kind);

                case "yyyy-MM-dd": return new DateTime(Convert.ToInt32(value.Substring(0, 4)), Convert.ToInt32(value.Substring(5, 2)), Convert.ToInt32(value.Substring(8, 2)), 0, 0, 0, kind);

                case "dd-MM-yyyy": return new DateTime(Convert.ToInt32(value.Substring(6, 4)), Convert.ToInt32(value.Substring(3, 2)), Convert.ToInt32(value.Substring(0, 2)), 0, 0, 0, kind);

                default:
                    throw new NotImplementedException();
            }
        }

        public static DateTime GetDateTime(
            string value,
            string format,
            DateTimeKind kind = DateTimeKind.Local)
        {
            switch (format)
            {
                case "yyyyMMdd HHmmss":
                    return new DateTime(
                        Convert.ToInt32(value.Substring(0, 4)),
                        Convert.ToInt32(value.Substring(4, 2)),
                        Convert.ToInt32(value.Substring(6, 2)),
                        Convert.ToInt32(value.Substring(9, 2)),
                        Convert.ToInt32(value.Substring(11, 2)),
                        Convert.ToInt32(value.Substring(13, 2)),
                        kind);

                default:
                    throw new NotImplementedException();
            }
        }

        public static bool CanConvertToDate(string value, string format)
        {
            bool canConvert = false;

            try
            {
                DateTime date = GetDate(value, format);
                canConvert = true;
            }
            catch { }

            return canConvert;
        }

        public static bool CanConvertToDateTime(string value, string format)
        {
            bool canConvert = false;

            try
            {
                DateTime date = GetDateTime(value, format);
                canConvert = true;
            }
            catch { }

            return canConvert;
        }

        /// <summary>
        /// Splits the date range in to pairs of consecutive date ranges of
        /// specified day duration. Ranges start at midnight and end at midnight
        /// except for first and list.
        /// </summary>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="daysPerRange"></param>
        /// <returns></returns>
        [Obsolete("Use " + nameof(SplitUTCDateRange) + " as it is more accurate")]
        public static List<DateTime[]> GetDateRanges(
            DateTime fromDateTime,
            DateTime toDateTime,
            int daysPerRange)
        {
            var ranges = new List<DateTime[]>();

            DateTime currentFromDateTime = fromDateTime.AddDays(-daysPerRange);
            DateTime currentToDateTime = currentFromDateTime;

            do
            {
                currentFromDateTime = currentFromDateTime.AddDays(daysPerRange);
                currentFromDateTime = new DateTime(
                    currentFromDateTime.Year,
                    currentFromDateTime.Month,
                    currentFromDateTime.Day,
                    0, 0, 0, 0,
                    DateTimeKind.Utc);

                currentToDateTime = currentFromDateTime.AddDays(daysPerRange).AddTicks(-1);

                if (currentFromDateTime < fromDateTime)
                {
                    currentFromDateTime = fromDateTime;
                }

                if (currentToDateTime > toDateTime)
                {
                    currentToDateTime = toDateTime;
                }

                ranges.Add(new DateTime[] { currentFromDateTime, currentToDateTime });
            } while (currentToDateTime < toDateTime);

            return ranges;
        }

        /// <summary>
        /// Splits the date range into consecutive UTC date ranges of specified
        /// number of days. Times are not considered.
        /// </summary>
        /// <param name="daysPerRange">The number of days in the range.
        /// The minimum is one day.</param>
        public static List<DateTimeRange> SplitUTCDateRange(
            DateTimeRange dateRange,
            int daysPerRange)
        {
            if (daysPerRange < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(daysPerRange),
                    "The range must be a minimum of one day."
                    );
            }

            DateTime rangeStart = dateRange.Start;
            DateTime rangeEnd = dateRange.End;

            var result = new List<DateTimeRange>();

            int daysInclusiveOfStartDate = daysPerRange - 1;

            do
            {
                if (rangeStart.Date.AddDays(daysInclusiveOfStartDate) > rangeEnd.Date)
                {
                    result.Add((rangeStart, rangeEnd));
                }
                else
                {
                    result.Add((rangeStart, OneDayFromStart(rangeStart, daysInclusiveOfStartDate)));
                }

                rangeStart = rangeStart.AddDays(daysInclusiveOfStartDate);
            } while (rangeStart < rangeEnd);

            return result;

            static DateTime OneDayFromStart(DateTime date, int value) =>
                date.AddDays(value).AddSeconds(-1);
        }

        /// <summary>
        /// Returns whether 2 date ranges overlap
        /// </summary>
        /// <param name="startDate1"></param>
        /// <param name="endDate1"></param>
        /// <param name="startDate2"></param>
        /// <param name="endDate2"></param>
        /// <returns></returns>
        public static bool IsRangesOverlap(DateTime startDate1, DateTime endDate1, DateTime startDate2, DateTime endDate2)
        {
            return (startDate1 >= startDate2 && startDate1 <= endDate2) ||     // #1 starts in #2
                (endDate1 >= startDate2 && endDate1 <= endDate2) ||  // #1 ends in #2
                (startDate2 >= startDate1 && startDate2 <= endDate1) ||  // #2 starts in #1
                (endDate2 >= startDate1 && endDate2 <= endDate1) || // #2 ends in #1
                (startDate1 < startDate2 && endDate1 > endDate2) || // #2 is inside #1
                (startDate2 < startDate1 && endDate2 > endDate1);   // #1 is inside #2
        }

        /// <summary>
        /// Returns whether the two TimeSpan(s) are the same value
        /// </summary>
        /// <param name="timeSpan1"></param>
        /// <param name="timeSpan2"></param>
        /// <returns></returns>
        public static bool IsSame(TimeSpan? timeSpan1, TimeSpan? timeSpan2)
        {
            if (timeSpan1 == null && timeSpan2 == null)
            {
                return true;
            }
            else if (timeSpan1 != null && timeSpan2 != null)
            {
                return timeSpan1.Value.Ticks == timeSpan2.Value.Ticks;
            }

            return false;
        }

        /// <summary>
        /// Returns whether the lists of DayOfWeek contain the same items
        /// </summary>
        /// <param name="daysOfWeek1"></param>
        /// <param name="daysOfWeek2"></param>
        /// <returns></returns>
        public static bool IsSame(List<DayOfWeek> daysOfWeek1, List<DayOfWeek> daysOfWeek2)
        {
            if (daysOfWeek1 == null && daysOfWeek2 == null)
            {
                return true;
            }
            else if (daysOfWeek1 != null && daysOfWeek2 != null && daysOfWeek1.Count == daysOfWeek2.Count)
            {
                var days1 = new StringBuilder();
                daysOfWeek1.OrderBy(dow => dow)
                    .ToList()
                    .ForEach(dow => days1.AppendFormat("|{0}", dow));

                var days2 = new StringBuilder();
                daysOfWeek2.OrderBy(dow => dow)
                    .ToList()
                    .ForEach(dow => days2.AppendFormat("|{0}", dow));

                return days1.ToString() == days2.ToString();
            }

            return false;
        }

        public static DateTime ConvertBroadcastToStandard(DateTime dateValue, TimeSpan timeValue)
        {
            bool isTimeAfterMidnight = timeValue.CompareTo(DefaultBroadcastDayStartTime) < 0;
            return dateValue.AddDays(isTimeAfterMidnight ? 1 : 0).Add(timeValue);
        }

        public static (DateTime broadcastDate, TimeSpan broadcastTime) ConvertStandardToBroadcast(DateTime dateValue)
        {
            bool isTimeAfterMidnight = dateValue.TimeOfDay.CompareTo(DefaultBroadcastDayStartTime) < 0;
            return (dateValue.Date.AddDays(isTimeAfterMidnight ? -1 : 0), dateValue.TimeOfDay);
        }

        private static readonly TimeSpan DefaultBroadcastDayStartTime = new TimeSpan(6, 0, 0);
        private static readonly TimeSpan DefaultBroadcastDayEndTime = new TimeSpan(5, 59, 59);

        public static DateTime CreateStartDateTime(DateTime usingTheDate, TimeSpan? havingTime = null)
        {
            TimeSpan startTime = havingTime ?? DefaultBroadcastDayStartTime;
            return usingTheDate.Date.Add(startTime);
        }

        public static DateTime CreateEndDateTime(DateTime usingTheDate, TimeSpan? havingTime = null)
        {
            TimeSpan endTime = havingTime ?? DefaultBroadcastDayEndTime;
            return usingTheDate.Date.AddDays(GetDaysToAddBasedOnEndTime(endTime)).Add(endTime);

            static int GetDaysToAddBasedOnEndTime(TimeSpan usingTheEndTime) =>
                IsTimePostMidnightAndPriorToBroadcastDayStartingTime(usingTheEndTime) ? 1 : 0;

            static bool IsTimePostMidnightAndPriorToBroadcastDayStartingTime(TimeSpan theTimeToCheck) =>
                theTimeToCheck.CompareTo(DefaultBroadcastDayStartTime) < 0;
        }
    }
}
