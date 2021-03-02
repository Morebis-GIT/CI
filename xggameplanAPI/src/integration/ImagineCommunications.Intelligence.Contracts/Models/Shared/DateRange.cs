using System;
using System.Diagnostics.Contracts;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared
{
    public class DateRange
    {
        public DateRange(DateTime start, DateTime end) =>
            (Start, End) = (start, end);

        public DateTime Start { get; }
        public DateTime End { get; }

        /// <summary>
        /// Determines whether the given <see cref="DateTime"/> is within the start and
        /// end of this <see cref="DateRange"/>. The start and end dates are inclusive.
        /// </summary>
        /// <param name="value">A <see cref="DateTime"/> to check.</param>
        /// <returns>Returns <see cref="true"/> if the date to check falls on or between
        /// the start and end dates of this <see cref="DateRange"/> instance.
        /// </returns>
        [Pure]
        public bool Contains(DateTime value) =>
            value.Date >= Start.Date && value.Date <= End.Date;
    }
}
