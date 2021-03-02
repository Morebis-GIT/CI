using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.DomainLogic
{
    /// <summary>
    /// Represents a clash exposure count calculator based on sales area, peak
    /// and off-peak times and other items.
    /// </summary>
    public class ClashExposureCountService
        : IClashExposureCountService
    {
        // Add one day because the broadcast day conversion might overflow the type.
        private static DateTime StartDateMin = DateTime.MinValue.AddDays(1d);

        // Subtract one day because the broadcast day conversion might overflow
        // the type.
        private static DateTime EndDateMax = DateTime.MaxValue.AddDays(-1d);

        /// <summary>
        /// <para>True if peak times are defined.</para>
        /// <para>
        /// Do not use the values of <see cref="_peakStartTime"/> or
        /// <see cref="_peakEndTime"/> if this is false.
        /// </para>
        /// </summary>
        private readonly bool _isPeakDefined;

        /// <summary>
        /// Do not use this value if <see cref="_isPeakDefined"/> is false.
        /// </summary>
        private readonly TimeSpan _peakStartTime;

        /// <summary>
        /// Do not use this value if <see cref="_isPeakDefined"/> is false.
        /// </summary>
        private readonly TimeSpan _peakEndTime;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="ClashExposureCountService"/> class.
        /// </summary>
        private ClashExposureCountService() =>
            _isPeakDefined = false;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="ClashExposureCountService"/> class with a specified peak time.
        /// </summary>
        /// <param name="peakTime"></param>
        private ClashExposureCountService(
            (TimeSpan start, TimeSpan end) peakTime
        ) => ((_peakStartTime, _peakEndTime), _isPeakDefined) = (peakTime, true);

        /// <summary>
        /// Calculate the effective clash exposure count for a date and sales area.
        /// </summary>
        /// <returns>
        /// Returns a value indicating the clash exposure count for a date and
        /// sales area.
        /// </returns>
        public int Calculate(
            IReadOnlyList<ClashDifference> clashDifferences,
            (int peak, int offPeak) clashExposureCountDefaults,
            (DateTime scheduledDate, string salesAreaName) dateAndSalesAreaName
            )
        {
            /**
             * In all cases, an empty column means "all".
            **/

            (DateTime scheduledDateTime, string salesAreaName) = dateAndSalesAreaName;

            IEnumerable<ClashDifference> possibleDifferences = KeepBreakSalesAreaDifferences(
                clashDifferences,
                salesAreaName
                );

            possibleDifferences = KeepBreakDateDifferences(
                possibleDifferences,
                scheduledDateTime.Date
                );

            possibleDifferences = KeepBreakTimeAndDayOfWeekDifferences(
                possibleDifferences,
                scheduledDateTime
                );

            ClashDifference selectedDifference = possibleDifferences.FirstOrDefault();

            (int peak, int offPeak) = clashExposureCountDefaults;

            (int peakExposureCount, int offPeakExposureCount) = (
                selectedDifference?.PeakExposureCount ?? peak,
                selectedDifference?.OffPeakExposureCount ?? offPeak
                );

            return IsDuringPeakTime(scheduledDateTime.TimeOfDay) ?
                peakExposureCount :
                offPeakExposureCount;

            //-----Local functions
            bool IsDuringPeakTime(TimeSpan time) =>
                _isPeakDefined &&
                _peakStartTime <= time && time <= _peakEndTime;

            //---------------------------------------------------------------------------
            // Keep only differences that match the break sales areas or all sales areas.
            IEnumerable<ClashDifference> KeepBreakSalesAreaDifferences(
                IEnumerable<ClashDifference> currentDifferences,
                string currentSalesAreaName
                )
            {
                var result = ForSpecificSalesArea(currentDifferences);

                return result.Any()
                    ? result
                    : ForAnySalesAreas(currentDifferences);

                //-----Local functions
                IEnumerable<ClashDifference> ForSpecificSalesArea(
                    IEnumerable<ClashDifference> diffs
                    ) =>
                    diffs.Where(i => i.SalesArea == currentSalesAreaName);

                IEnumerable<ClashDifference> ForAnySalesAreas(
                    IEnumerable<ClashDifference> diffs
                    ) =>
                    diffs.Where(i => String.IsNullOrWhiteSpace(i.SalesArea));
            }

            //---------------------------------------------------------
            // Keep only differences that match the scheduled start's date.
            IEnumerable<ClashDifference> KeepBreakDateDifferences(
                IEnumerable<ClashDifference> currentDifferences,
                DateTime scheduledDate
                )
            {
                foreach (var diff in currentDifferences)
                {
                    DateTimeRange differencePeriod = NormaliseDifferencePeriodDates(
                        diff.StartDate,
                        diff.EndDate
                        );

                    if (differencePeriod.Contains(scheduledDate.Date))
                    {
                        yield return diff;
                    }
                }
            }

            //-----------------------------------------------------------------
            // Ensure the difference period dates have values and form a range.
            DateTimeRange NormaliseDifferencePeriodDates(
                DateTime? diffStartDate,
                DateTime? diffEndDate
                )
            {
                DateTime startDate = IsForAnyDate(diffStartDate)
                    ? StartDateMin
                    : diffStartDate.Value;

                DateTime endDate = IsForAnyDate(diffEndDate)
                    ? EndDateMax
                    : diffEndDate.Value;

                return (
                    startDate.Date,
                    endDate.Date
                    );

                //-----Local functions
                bool IsForAnyDate(DateTime? value) =>
                    !value.HasValue;
            }

            //------------------------------------------------------------------------------------
            // Keep only differences that match the break's start time of day and day of the week.
            IEnumerable<ClashDifference> KeepBreakTimeAndDayOfWeekDifferences(
                IEnumerable<ClashDifference> currentDifferences,
                DateTime scheduledDate
                )
            {
                DayOfWeek breakDayOfWeek = scheduledDate.DayOfWeek;

                foreach (var diff in currentDifferences)
                {
                    var differenceTimeAndDaysOfWeek = diff.TimeAndDow?.ConvertToTimeAndDow();

                    if (AnyTimeAndDayOfTheWeek(differenceTimeAndDaysOfWeek))
                    {
                        yield return diff;
                        continue;
                    }

                    bool differenceIsValid;

                    if (AnyDayOfTheWeek(differenceTimeAndDaysOfWeek.DaysOfWeek))
                    {
                        differenceIsValid = true;
                    }
                    else
                    {
                        differenceIsValid = differenceTimeAndDaysOfWeek.Contains(breakDayOfWeek);
                    }

                    differenceIsValid &= CompareTimes(diff, differenceTimeAndDaysOfWeek);

                    if (differenceIsValid)
                    {
                        yield return diff;
                    }
                }

                //-----Local functions
                bool AnyTimeAndDayOfTheWeek(TimeAndDow value) =>
                    value is null;

                bool AnyDayOfTheWeek(string value) =>
                    String.IsNullOrWhiteSpace(value);

                bool CompareTimes(
                    ClashDifference diff,
                    TimeAndDow differenceTimeAndDaysOfWeek)
                {
                    DateTimeRange differencePeriod = NormaliseDifferencePeriodDates(
                        diff.StartDate,
                        diff.EndDate
                        );

                    var differenceIsValid = true;

                    if (differenceTimeAndDaysOfWeek.StartTime.HasValue)
                    {
                        var normalisedAllowedPeriodStart =
                            differencePeriod.Start.Date.Add(
                                differenceTimeAndDaysOfWeek.StartTime.Value
                                );

                        if (normalisedAllowedPeriodStart.Date == StartDateMin)
                        {
                            differenceIsValid &= normalisedAllowedPeriodStart.TimeOfDay <= scheduledDate.TimeOfDay;
                        }
                        else
                        {
                            differenceIsValid &= normalisedAllowedPeriodStart <= scheduledDate;
                        }
                    }

                    if (differenceTimeAndDaysOfWeek.EndTime.HasValue)
                    {
                        var normalisedAllowedPeriodEnd =
                            differencePeriod.End.Date.Add(
                                differenceTimeAndDaysOfWeek.EndTime.Value
                            );

                        if (normalisedAllowedPeriodEnd.Date == EndDateMax)
                        {
                            differenceIsValid &= scheduledDate.TimeOfDay <= normalisedAllowedPeriodEnd.TimeOfDay;
                        }
                        else
                        {
                            differenceIsValid &= scheduledDate <= normalisedAllowedPeriodEnd;
                        }
                    }

                    return differenceIsValid;
                }
            }
        }

        /// <summary>
        /// Create a reference to an object that calculates a clash exposure
        /// count when all day is Off Peak.
        /// </summary>
        public static IClashExposureCountService Create() =>
            new ClashExposureCountService();

        /// <summary>
        /// Create a reference to an object that calculates a clash exposure
        /// count where the peak time of day is defined between the given start
        /// and end times, inclusive. All other times are off peak.
        /// </summary>
        public static IClashExposureCountService Create(
            (string peakStartTime, string peakEndTime) peakStartAndEnd
            )
        {
            const string PeakTimeFormat = "hhmmss";

            (string peakStartValue, string peakEndValue) = peakStartAndEnd;

            if (TimeSpan.TryParseExact(peakStartValue, PeakTimeFormat, CultureInfo.InvariantCulture, out TimeSpan pStart) &&
                TimeSpan.TryParseExact(peakEndValue, PeakTimeFormat, CultureInfo.InvariantCulture, out TimeSpan pEnd))
            {
                return new ClashExposureCountService((pStart, pEnd));
            }

            return Create();
        }
    }
}
