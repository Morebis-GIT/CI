using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Spots;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public static class ProgrammeSpecificDataSet
    {
        /// <summary>
        /// Get a collection of objects that fall within a programme's start and end.
        /// </summary>
        /// <typeparam name="T">Can only be Break or Spot at the minute.</typeparam>
        /// <param name="programmeDateTimes"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<T> GetForPeriod<T>(
            DateTimeRange programmeDateTimes,
            IReadOnlyCollection<T> source
            )
            where T : class
        {
            switch (source)
            {
                case IReadOnlyCollection<Break> breaks:
                    return (IReadOnlyCollection<T>)GetProgrammeBreaks(programmeDateTimes, breaks);

                case IReadOnlyCollection<Spot> spots:
                    return (IReadOnlyCollection<T>)GetProgrammeSpots(programmeDateTimes, spots);

                default:
                    throw new InvalidOperationException("Only breaks and spots are currently valid.");
            }
        }

        private static IReadOnlyCollection<Break> GetProgrammeBreaks(
            DateTimeRange programmeDateTimes,
            IReadOnlyCollection<Break> breaks)
        {
            var (programmeStart, programmeEnd) = programmeDateTimes;

            return breaks.Where(b =>
                b.ScheduledDate >= programmeStart &&
                b.ScheduledDate.Add(b.Duration.ToTimeSpan()) <= programmeEnd)
                .OrderBy(b => b.ScheduledDate)
                .ToList();
        }

        private static IReadOnlyCollection<Spot> GetProgrammeSpots(
            DateTimeRange programmeDateTimes,
            IReadOnlyCollection<Spot> spots)
        {
            var (programmeStart, programmeEnd) = programmeDateTimes;

            return spots.Where(s =>
                s.StartDateTime >= programmeStart &&
                s.StartDateTime.Add(s.SpotLength.ToTimeSpan()) <= programmeEnd)
                .OrderBy(p => p.StartDateTime)
                .ToList();
        }
    }
}
