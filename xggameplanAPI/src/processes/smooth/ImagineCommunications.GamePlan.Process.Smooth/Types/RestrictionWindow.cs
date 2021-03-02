using System;
using System.Diagnostics.Contracts;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;

namespace ImagineCommunications.GamePlan.Process.Smooth.Types
{
    /// <summary>
    /// Defines a date and time window in which a restriction is valid.
    /// </summary>
    public class RestrictionWindow
    {
        private (DateTime start, DateTime? end) _windowDate { get; }
        private (TimeSpan? start, TimeSpan? end) _windowTime { get; }

        public RestrictionWindow(
            (DateTime start, DateTime? end) windowDate,
            (TimeSpan? start, TimeSpan? end) windowTime
            ) =>
            (_windowDate, _windowTime) = (windowDate, windowTime);

        /// <summary>
        /// Determines whether the restriction window contains a scheduled break.
        /// </summary>
        /// <param name="breakSchedule">A schedule to seek.</param>
        /// <returns>
        /// Returns a value indicating whether the specified break schedule occurs
        /// within the current restriction window.
        /// </returns>
        [Pure]
        public bool Contains(DateTime breakSchedule)
        {
            DateTime windowStartDate = _windowDate.start.Date;

            // Somehow a null gets passed in (it shouldn't but it can).
            if (windowStartDate == DateTime.MinValue)
            {
                return false;
            }

            DateTime windowEndDate = _windowDate.end?.Date ?? DateTime.MaxValue;

            var breakDate = breakSchedule.Date;
            var restrictionDateWindow = new DateRange(windowStartDate, windowEndDate);

            if (!restrictionDateWindow.Contains(breakDate))
            {
                return false;
            }

            if (!_windowTime.start.HasValue)
            {
                return true;
            }

            TimeSpan restrictionStartTime = _windowTime.start.Value;
            var breakTime = breakSchedule.TimeOfDay;

            if (breakTime < restrictionStartTime)
            {
                return false;
            }

            if (breakTime == restrictionStartTime)
            {
                return true;
            }

            if (_windowTime.end.HasValue)
            {
                TimeSpan restrictionEndTime = _windowTime.end.Value;
                if (breakTime == restrictionEndTime)
                {
                    return true;
                }

                if (breakTime > restrictionEndTime)
                {
                    return false;
                }

                return true;
            }
            else
            {
                /*
                 * The restriction is from the start time to the end of the programme.
                 * Don't know the programme end time here so must *at this point* be
                 * inside the restriction time frame.
                 */
                return true;
            }
        }
    }
}
