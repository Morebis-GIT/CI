using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public static class SpotUtilities
    {
        /// <summary>
        /// Returns total length of all spots in list.
        /// </summary>
        /// <param name="spots"></param>
        /// <returns></returns>
        public static TimeSpan GetTotalSpotLength(IReadOnlyCollection<Spot> spots)
        {
            if (spots.Count == 0)
            {
                return TimeSpan.Zero;
            }

            var total = new Duration();

            foreach (var spot in spots)
            {
                total = total.Plus(spot.SpotLength);
            }

            return total.ToTimeSpan();
        }

        /// <summary>
        /// Returns average length of all spots in list.
        /// </summary>
        /// <param name="spots"></param>
        /// <returns></returns>
        public static TimeSpan GetAverageSpotLength(IReadOnlyCollection<Spot> spots)
        {
            if (spots.Count == 0)
            {
                return TimeSpan.Zero;
            }

            TimeSpan total = GetTotalSpotLength(spots);

            return TimeSpan.FromTicks(total.Ticks / spots.Count);
        }

        /// <summary>
        /// Gets spot details list string
        /// </summary>
        /// <param name="spots"></param>
        /// <returns></returns>
        public static string GetSpotDetailsListString(IReadOnlyCollection<Spot> spots)
        {
            var spotList = new StringBuilder();

            foreach (var spot in spots)
            {
                if (spotList.Length > 0)
                {
                    _ = spotList.Append("; ");
                }

                if (String.IsNullOrEmpty(spot.RequestedPositioninBreak))
                {
                    _ = spotList.Append(spot.ExternalSpotRef);
                }
                else
                {
                    _ = spotList.AppendFormat("{0} [SP={1}; BR={2}; RPIB={3}; PL={4}]", spot.ExternalSpotRef, spot.Sponsored, spot.BreakRequest, spot.RequestedPositioninBreak, spot.Preemptlevel);
                }
            }

            return spotList.ToString();
        }

        /// <summary>
        /// Concatenates the <see cref="Spot.ExternalSpotRef"/> values of a
        /// specified array or the members of a collection, using the specified
        /// separator between each element or member.
        /// </summary>
        /// <param name="separator">
        /// The string to use as a separator. <paramref name="separator"/> is
        /// included in the returned string only if <paramref name="spots"/> has
        /// more than one element.
        /// </param>
        /// <param name="spots">A collection that contains the spots.</param>
        /// <returns>
        /// A string that consists of the members of values delimited by the
        /// separator string.
        /// </returns>
        public static string GetListOfSpotExternalReferences(
            string separator,
            IReadOnlyCollection<Spot> spots
            ) => String.Join(separator, spots.Select(b => b.ExternalSpotRef));

        /// <summary>
        /// Whether spot has restricted spot times
        /// </summary>
        public static bool HasRestrictedSpotTimes(
            DateTime spotStartDateTime,
            DateTime spotEndDateTime,
            DateTime programmeStartDateTime,
            Duration programmeDuration
            )
        {
            if (spotStartDateTime != programmeStartDateTime ||
                (spotEndDateTime.Year >= 1900 && spotEndDateTime != programmeStartDateTime.Add(programmeDuration.ToTimeSpan()))
            )
            {
                return true;
            }

            return false;
        }

        public static List<Spot> GetSpotsWithSameSponsor(
            IReadOnlyCollection<Spot> spotsForSponsor,
            IReadOnlyCollection<Spot> spotsToCheck,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos
            )
        {
            var spotsToReturn = new List<Spot>();

            foreach (Spot spot in spotsForSponsor)
            {
                foreach (Spot currentSpot in spotsToCheck.Where(s => s.Sponsored))
                {
                    if (spotsToReturn.Contains(currentSpot))
                    {
                        continue;
                    }

                    if (IsSameSpotSponsor(currentSpot, spot, spotInfos))
                    {
                        spotsToReturn.Add(currentSpot);
                    }
                }
            }

            return spotsToReturn;
        }

        /// <summary>
        /// Determines whether this Spot instance can be placed within the requested
        /// break or container.
        /// </summary>
        /// <param name="spot">The spot.</param>
        /// <param name="progSmoothBreaks">The prog smooth breaks.</param>
        /// <param name="breakPositionRules">The break position rules.</param>
        /// <param name="respectSpotTime">if set to <c>true</c>, respect spot time.</param>
        /// <param name="validBreaksForSpotTime">The valid breaks for spot time.</param>
        /// <param name="isRestrictedSpotTime">if set to <c>true</c>, is restricted spot time.</param>
        /// <param name="progSmoothBreak">The prog smooth break.</param>
        /// <returns>
        ///   <c>true</c> if this Spot instance can be placed within the break or container requested; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanSpotBePlacedInRequestedBreakOrContainer(
            Spot spot,
            IReadOnlyList<SmoothBreak> progSmoothBreaks,
            SpotPositionRules breakPositionRules,
            bool respectSpotTime,
            IOrderedEnumerable<SmoothBreak> validBreaksForSpotTime,
            bool isRestrictedSpotTime,
            SmoothBreak progSmoothBreak)
        {
            var canAddSpotService = CanAddSpotService.Factory(progSmoothBreak);

            string spotBreakOrContainerRequest = spot.BreakRequest;
            if (ContainerReference.TryParse(spotBreakOrContainerRequest, out ContainerReference cr))
            {
                spotBreakOrContainerRequest = cr.ToString();
            }

            if (canAddSpotService.CanAddSpotWithBreakRequest(spotBreakOrContainerRequest, progSmoothBreaks, breakPositionRules))
            {
                return true;
            }
            else if (IsBreakWithinSpotTimeRestriction(respectSpotTime, isRestrictedSpotTime, validBreaksForSpotTime, progSmoothBreak))
            {
                return true;
            }

            return false;
        }

        internal static bool IsBreakWithinSpotTimeRestriction(
            bool respectSpotTime,
            bool isRestrictedSpotTime,
            IOrderedEnumerable<SmoothBreak> validBreaksForSpotTime,
            SmoothBreak progSmoothBreak)
        {
            return isRestrictedSpotTime
                && respectSpotTime
                && progSmoothBreak.Position >= validBreaksForSpotTime.First().Position
                && progSmoothBreak.Position <= validBreaksForSpotTime.LastOrDefault()?.Position;
        }

        /// <summary>
        /// <para>Returns whether spots are for same sponsor.</para>
        /// </summary>
        public static bool IsSameSpotSponsor(
            Spot spot1,
            Spot spot2,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos)
        {
            SpotInfo spot1Info = spotInfos[spot1.Uid];
            SpotInfo spot2Info = spotInfos[spot2.Uid];

            return
                HasAdvertiser(spot1Info) &&
                HasAdvertiser(spot2Info) &&
                AdvertisersAreTheSame(spot1Info, spot2Info);
        }

        private static bool HasAdvertiser(SpotInfo spotInfo) =>
            !String.IsNullOrEmpty(spotInfo.ProductAdvertiserIdentifier);

        private static bool AdvertisersAreTheSame(
            SpotInfo spot1Info,
            SpotInfo spot2Info) =>
            spot1Info.ProductAdvertiserIdentifier == spot2Info.ProductAdvertiserIdentifier;
    }
}
