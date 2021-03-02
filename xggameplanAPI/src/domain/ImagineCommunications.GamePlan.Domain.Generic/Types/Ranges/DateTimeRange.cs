using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges
{
    /// <summary>
    /// Represents an immutable range of date and times, start and end inclusive.
    /// </summary>
    [DebuggerDisplay("{Start.ToUniversalTime().ToString(\"O\"), nq} to {End.ToUniversalTime().ToString(\"O\"), nq}")]
    public readonly struct DateTimeRange : IEquatable<DateTimeRange>, IEnumerable<DateTime>
    {
        #region DateTimeRange Types

        public enum CompareStrategy
        {
            IgnoreEdges = 0,
            IncludeEdges = 1
        }

        #endregion DateTimeRange Types

        public DateTime Start { get; }
        public DateTime End { get; }

        /// <summary>
        /// Instantiates a new instance of the <see cref="DateTimeRange"/> structure to the
        /// specified start and end date and times.
        /// </summary>
        /// <param name="start">The start date.</param>
        /// <param name="end">The end date.</param>
        public DateTimeRange(DateTime start, DateTime end) => (Start, End) = (start, end);

        /// <summary>
        /// Determines whether the given <see cref="DateTime"/> is within the start and
        /// end of this <see cref="DateTimeRange"/>. The start and end dates are inclusive.
        /// </summary>
        /// <param name="value">A <see cref="DateTime"/> to check.</param>
        /// <returns>Returns <c>true</c> if the date to check falls on or between
        /// the start and end dates of this <see cref="DateTimeRange"/> instance.
        /// </returns>
        public bool Contains(DateTime value) => value >= Start && value <= End;

        public override bool Equals(object obj) => obj is DateTimeRange dtr && Equals(dtr);

        public bool Equals(DateTimeRange other) => Start == other.Start && End == other.End;

        public override int GetHashCode()
        {
            int hashCode = -1676728671;
            hashCode = hashCode * -1521134295 + Start.GetHashCode();
            hashCode = hashCode * -1521134295 + End.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Checks whether <see cref="DateTimeRange"/> structures has overlay
        /// <para> </para>
        /// <para> If it is not that : </para>
        /// <para> 1. 'This' range is completely after other; </para>
        /// <para> 2. 'This' range is completely before other; </para>
        /// <para> Then overlay exists</para>
        /// </summary>
        /// <param name="other"></param>
        /// <param name="compareStrategy"></param>
        /// <returns></returns>
        public bool Overlays(
            in DateTimeRange other,
            CompareStrategy compareStrategy = CompareStrategy.IncludeEdges)
        {
            (DateTime start, DateTime end) = other;

            switch (compareStrategy)
            {
                case CompareStrategy.IncludeEdges:
                    return Start <= end && End >= start;

                case CompareStrategy.IgnoreEdges:
                    return Start < end && End > start;

                default:
                    goto case CompareStrategy.IncludeEdges;
            }
        }

        public IEnumerator<DateTime> GetEnumerator()
        {
            DateTime start = Start;
            DateTime end = End;

            for (DateTime currentDateTime = start; currentDateTime <= end; currentDateTime = currentDateTime.AddDays(1))
            {
                yield return currentDateTime;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static bool operator ==(DateTimeRange range1, DateTimeRange range2) => range1.Equals(range2);

        public static bool operator !=(DateTimeRange range1, DateTimeRange range2) => !(range1 == range2);

        public static implicit operator DateTimeRange((DateTime start, DateTime end) value) =>
            new DateTimeRange(value.start, value.end);

        public void Deconstruct(out DateTime start, out DateTime end) => (start, end) = (Start, End);

        public override string ToString() =>
            $"{Start.ToString("O", CultureInfo.InvariantCulture)} to {End.ToString("O", CultureInfo.InvariantCulture)}";

        public static DateTimeRange ToDateTimeRange((DateTime start, DateTime end) value) =>
            new DateTimeRange(value.start, value.end);

        public static DateTimeRange ToDateTimeRange(DateTime start, DateTime end) =>
            new DateTimeRange(start, end);
    }
}
