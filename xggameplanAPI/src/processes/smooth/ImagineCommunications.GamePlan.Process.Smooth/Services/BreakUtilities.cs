using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using xggameplan.Common;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public static class BreakUtilities
    {
        /// <summary>
        /// Gets the ratings schedule date for the break based on the ratings frequency
        /// </summary>
        public static DateTime GetRatingScheduleDateTimeForBreak(
            DateTime breakScheduleDate,
            TimeSpan ratingsFrequency)
        {
            var scheduleDateTime = new DateTime(
                breakScheduleDate.Year,
                breakScheduleDate.Month,
                breakScheduleDate.Day,
                0,
                0,
                0,
                DateTimeKind.Utc)
            .AddTicks(-ratingsFrequency.Ticks);

            while (true)
            {
                scheduleDateTime = scheduleDateTime.Add(ratingsFrequency);
                if (scheduleDateTime == breakScheduleDate)
                {
                    return scheduleDateTime;
                }

                if (scheduleDateTime > breakScheduleDate)
                {
                    return scheduleDateTime.AddTicks(-ratingsFrequency.Ticks);
                }
            }
        }

        public static bool IsBreakInProgramme(Break aBreak, Programme programme)
        {
            if (aBreak is null)
            {
                throw new ArgumentNullException(nameof(aBreak));
            }

            if (programme is null)
            {
                throw new ArgumentNullException(nameof(programme));
            }

            return
                aBreak.ScheduledDate >= programme.StartDateTime &&
                aBreak.ScheduledDate.Add(aBreak.Duration.ToTimeSpan()) <= programme.StartDateTime.Add(programme.Duration.ToTimeSpan());
        }

        /// <summary>
        /// Get all linked spots that have been placed in this break. Returns
        /// related multipart spots in the correct processing order. E.g. TOP
        /// then TAIL. The returned list will include the input spot.
        /// </summary>
        public static IReadOnlyCollection<Spot> GetLinkedMultipartSpots(
            Spot spot,
            IReadOnlyCollection<Spot> spots,
            bool includeInputSpotInOutput)
        {
            if (!spot.IsMultipartSpot)
            {
                throw new ArgumentException(
                    $"Spot {spot.ExternalSpotRef} is not a multipart spot");
            }

            var linkedSpots = new List<Spot>();
            Dictionary<string, short> positionOrder = null;

            bool IsMultipartSpotsPart(Spot left, Spot right) =>
                left.IsMultipartSpot
                && left.MultipartSpot == right.MultipartSpot;

            switch (spot.MultipartSpot)
            {
                case MultipartSpotTypes.TopTail:
                    foreach (var currentSpot in spots
                        .Where(s => IsMultipartSpotsPart(s, spot)))
                    {
                        if (spot == currentSpot)
                        {
                            // Input spot is in spots list
                            if (includeInputSpotInOutput)
                            {
                                linkedSpots.Add(spot);
                            }
                        }
                        else if (currentSpot.MultipartSpotRef == spot.ExternalSpotRef
                            || currentSpot.ExternalSpotRef == spot.MultipartSpotRef)
                        {
                            linkedSpots.Add(currentSpot);
                        }
                    }

                    if (!linkedSpots.Contains(spot) && includeInputSpotInOutput)
                    {
                        linkedSpots.Add(spot);
                    }

                    positionOrder = new Dictionary<string, short>()
                    {
                        { MultipartSpotPositions.TopTail_Top, 1 },
                        { MultipartSpotPositions.TopTail_Tail, 2 }
                    };

                    break;

                case MultipartSpotTypes.SameBreak:
                    foreach (var currentSpot in spots
                        .Where(s => IsMultipartSpotsPart(s, spot)))
                    {
                        if (spot == currentSpot)    // Input spot is in spots list
                        {
                            if (includeInputSpotInOutput)
                            {
                                linkedSpots.Add(spot);
                            }
                        }
                        else if (currentSpot.MultipartSpotRefs.Contains(spot.ExternalSpotRef)
                            || spot.MultipartSpotRefs.Intersect(currentSpot.MultipartSpotRefs).Any()
                            || spot.MultipartSpotRefs.Contains(currentSpot.ExternalSpotRef))
                        {
                            linkedSpots.Add(currentSpot);
                        }
                    }

                    if (!linkedSpots.Contains(spot) && includeInputSpotInOutput)
                    {
                        linkedSpots.Add(spot);
                    }

                    positionOrder = new Dictionary<string, short>()
                    {
                        { MultipartSpotPositions.SameBreak_Top, 1 },
                        { MultipartSpotPositions.SameBreak_Mid, 2 },
                        { MultipartSpotPositions.SameBreak_Tail, 3 },
                        { MultipartSpotPositions.SameBreak_Any, 4 }
                    };
                    break;

                default:
                    return linkedSpots;
            }

            return linkedSpots
                .OrderBy(s => positionOrder.ContainsKey(s.MultipartSpotPosition)
                    ? positionOrder[s.MultipartSpotPosition]
                    : Int16.MaxValue)
                .ToList();
        }

        public static bool IsBreakRefNotSetOrUnused(string externalBreakRef) =>
            String.IsNullOrEmpty(externalBreakRef) ||
            String.Equals(externalBreakRef, Globals.UnplacedBreakString, StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Concatenates the <see cref="Break.ExternalBreakRef"/> values of a
        /// specified array or the members of a collection, using the specified
        /// separator between each element or member.
        /// </summary>
        /// <param name="separator">
        /// The string to use as a separator. <paramref name="separator"/> is
        /// included in the returned string only if <paramref name="breaks"/>
        /// has more than one element.
        /// </param>
        /// <param name="breaks">A collection that contains the breaks.</param>
        /// <returns>
        /// A string that consists of the members of values delimited by the
        /// separator string.
        /// </returns>
        public static string GetListOfBreakExternalReferences(
            string separator,
            IReadOnlyCollection<Break> breaks) =>
            String.Join(separator, breaks.Select(b => b.ExternalBreakRef));
    }
}
