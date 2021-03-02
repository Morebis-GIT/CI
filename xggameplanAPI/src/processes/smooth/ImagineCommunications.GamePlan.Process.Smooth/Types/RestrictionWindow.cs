using System;
using System.Diagnostics.Contracts;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;

namespace ImagineCommunications.GamePlan.Process.Smooth.Types
{
    /// <summary>
    /// Defines a date and time window in which a restriction is valid.
    /// </summary>
    public readonly struct RestrictionWindow
    {
        private readonly (DateTime start, DateTime? end) _windowDate;
        private readonly (TimeSpan? start, TimeSpan? end) _windowTime;

        public RestrictionWindow(
            (DateTime start, DateTime? end) windowDate,
            (TimeSpan? start, TimeSpan? end) windowTime
            ) =>
            (_windowDate, _windowTime) = (windowDate, windowTime);

        /// <summary>
        /// Determines whether the restriction window contains a scheduled break.
        /// </summary>
        /// <param name="breakSchedule">A schedule to check.</param>
        /// <returns>
        /// <para>Returns a value indicating whether the specified break schedule
        /// occurs within the current restriction window.</para>
        /// <para>Rules</para>
        /// <list type="bullet">
        /// <item><description>
        /// Restrictions must have a start date.
        /// </description></item>
        /// <item><description>
        /// End dates are optional. If no end date is given it will default to the
        /// Landmark default end date, 31 December 2037.
        /// </description></item>
        /// <item><description>
        /// Times are optional. If not given the restriction is date based only.
        /// </description></item>
        /// <item><description>
        /// Start time may be present without an end time. If no end time is
        /// given it should default to the end of the day.
        /// </description></item>
        /// <item><description>
        /// End times are not allowed without a start time.
        /// </description></item>
        /// <item><description>
        /// Start and end times are taken as a time span for each day of the
        /// date window. So, for example, Monday to Thursday, 5pm to 7pm each
        /// day, not Monday 5pm all the way to 7pm on Thursday.
        /// </description></item>
        /// </list>
        /// </returns>
        [Pure]
        public bool Contains(DateTime breakSchedule)
        {
            // CS1673 Local functions inside structs cannot access "this".
            var me = this;

            // Cover instances where the start date is null.
            // This should never happen but somehow does...
            if (!RestrictionStartDateIsValid())
            {
                return false;
            }

            DateTime restrictionStartDate = GetRestrictionStartDate();
            DateTime restrictionEndDate = GetRestrictionEndDate();

            var restrictionDates = new DateRange(restrictionStartDate, restrictionEndDate);
            if (!restrictionDates.Contains(breakSchedule.Date))
            {
                return false;
            }

            if (OnlyInterestedInDates())
            {
                return true;
            }

            // Narrow down the focus to include times within the date range.
            var restrictionStartTime = GetRestrictionStartTime();
            var restrictionEndTime = GetRestrictionEndTime();

            var breakTime = breakSchedule.TimeOfDay;
            return DoesRestrictionContainBreak(
                restrictionStartTime,
                restrictionEndTime,
                breakTime);

            //-----------------
            // Local functions
            bool RestrictionStartDateIsValid() => me._windowDate.start.Date != DateTime.MinValue;
            bool OnlyInterestedInDates() => !me._windowTime.start.HasValue;

            DateTime GetRestrictionStartDate() => me._windowDate.start.Date;
            DateTime GetRestrictionEndDate() => me._windowDate.end?.Date ?? LandmarkDefaultEndDate;
            TimeSpan GetRestrictionStartTime() => me._windowTime.start.Value;

            TimeSpan GetRestrictionEndTime() =>
                me._windowTime.end ?? TimeSpan.FromTicks(TimeSpan.TicksPerDay - 1);

            static bool DoesRestrictionContainBreak(
                TimeSpan restrictionStartTime,
                TimeSpan restrictionEndTime,
                TimeSpan breakTime)
            {
                return restrictionStartTime <= breakTime &&
                    breakTime <= restrictionEndTime;
            }
        }

        private static DateTime LandmarkDefaultEndDate => new DateTime(2037, 12, 31);
    }
}
