using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public class ProductClashChecker
        : IProductClashChecker
    {
        private readonly bool _enabled = false;

        public ProductClashChecker(bool enabled) => _enabled = enabled;

        /// <summary>
        /// Returns spots that clash with any of the proposed spots, either at parent or child level.
        /// </summary>
        /// <param name="spotCollection">The spots to be checked for clashes.</param>
        /// <param name="spotsAlreadyInBreak">A collection of spots already placed in a break. The
        /// new spot is checked for clashes with any of these.</param>
        /// <param name="spotInfos">The SpotInfo covering all of the spots passed in to this
        /// method.</param>
        /// <param name="clashLevel">The product category level check, either child or parent.</param>
        /// <seealso cref="ClashCodeLevel"/>
        public IReadOnlyCollection<Spot> GetProductClashesForMultipleSpots(
            IReadOnlyCollection<Spot> spotCollection,
            IReadOnlyCollection<Spot> spotsAlreadyInBreak,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            ClashCodeLevel clashLevel
            )
        {
            if (!_enabled)
            {
                return new List<Spot>();
            }

            var spotClashes = new List<Spot>();

            foreach (var spot in spotCollection)
            {
                IReadOnlyCollection<Spot> spotClashesForSpot = GetProductClashesForSingleSpot(
                    spot,
                    spotsAlreadyInBreak,
                    spotInfos,
                    clashLevel);

                spotClashes.AddRange(
                    TakeUniqueSpotClashes(spotClashesForSpot)
                );
            }

            return spotClashes;

            IEnumerable<Spot> TakeUniqueSpotClashes(IEnumerable<Spot> spotClashesForSpot) =>
                spotClashesForSpot.Where(s => !spotClashes.Contains(s));
        }

        /// <summary>
        /// Returns used spots that clash with proposed spot, either at parent or
        /// child level.
        /// </summary>
        /// <param name="spot">The spot to be checked for clashes.</param>
        /// <param name="spotsAlreadyInBreak">A collection of spots already placed
        /// in a break. The new spot is checked for clashes with any of these.</param>
        /// <param name="spotInfos">The SpotInfo covering all of the spots passed
        /// in to this method.</param>
        /// <param name="clashLevel">The product category level check, either
        /// child or parent.</param>
        /// <seealso cref="ClashCodeLevel"/>
        public IReadOnlyCollection<Spot> GetProductClashesForSingleSpot(
            Spot spot,
            IReadOnlyCollection<Spot> spotsAlreadyInBreak,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            ClashCodeLevel clashLevel
            )
        {
            if (!_enabled)
            {
                return new List<Spot>();
            }

            if (spot is null)
            {
                throw new ArgumentNullException(nameof(spot));
            }

            if (spotInfos is null)
            {
                throw new ArgumentNullException(nameof(spotInfos));
            }

            if (!spotInfos.TryGetValue(spot.Uid, out SpotInfo proposedSpotInfo))
            {
                return new List<Spot>();
            }

            switch (clashLevel)
            {
                case ClashCodeLevel.Parent:
                    return GetParentProductClashes(
                        spot,
                        proposedSpotInfo.ParentProductClashCode,
                        spotsAlreadyInBreak,
                        spotInfos
                        );

                case ClashCodeLevel.Child:
                    return GetChildProductClashes(
                        spot,
                        proposedSpotInfo.ProductClashCode,
                        spotsAlreadyInBreak,
                        spotInfos
                        );

                default:
                    throw new InvalidOperationException(
                        $"An unknown value for {nameof(ClashCodeLevel)} with the value {clashLevel} was found."
                        );
            }
        }

        private IReadOnlyList<Spot> GetProductClashes_Imp(
            Spot proposedSpot,
            IReadOnlyCollection<Spot> spotsUsed,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfoCollection,
            Func<SpotInfo, bool> isProductClashFunc
            )
        {
            var spotClashes = new List<Spot>();

            var externalSpotRefsToIgnore = ExternalSpotRefsOfMultipartSpotsLinkedToSibling(
                proposedSpot,
                spotsUsed);

            foreach (Spot spotUsed in spotsUsed.Where(s =>
                spotInfoCollection.ContainsKey(s.Uid)
                && !externalSpotRefsToIgnore.Contains(s.ExternalSpotRef))
                )
            {
                if (isProductClashFunc(spotInfoCollection[spotUsed.Uid]))
                {
                    spotClashes.Add(spotUsed);
                }
            }

            return spotClashes;
        }

        /// <summary>
        /// Get product clashes at the parent product category level
        /// </summary>
        private IReadOnlyList<Spot> GetParentProductClashes(
            Spot proposedSpot,
            string parentProductClashCode,
            IReadOnlyCollection<Spot> spotsUsed,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfoCollection
            )
        {
            if (String.IsNullOrEmpty(parentProductClashCode))
            {
                return new List<Spot>();
            }

            bool IsParentProductClash(SpotInfo otherSpotInfo) =>
                parentProductClashCode == otherSpotInfo?.ParentProductClashCode;

            var spotClashes = GetProductClashes_Imp(
                proposedSpot,
                spotsUsed,
                spotInfoCollection,
                IsParentProductClash
                );

            return spotClashes;
        }

        /// <summary>
        /// Get product clashes at the child product category level
        /// </summary>
        private IReadOnlyList<Spot> GetChildProductClashes(
            Spot proposedSpot,
            string productClashCode,
            IReadOnlyCollection<Spot> spotsUsed,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfoCollection
            )
        {
            if (String.IsNullOrEmpty(productClashCode))
            {
                return new List<Spot>();
            }

            bool IsProductClash(SpotInfo otherSpotInfo) =>
                productClashCode == otherSpotInfo?.ProductClashCode;

            var spotClashes = GetProductClashes_Imp(
                proposedSpot,
                spotsUsed,
                spotInfoCollection,
                IsProductClash
                );

            return spotClashes;
        }

        private static IReadOnlyCollection<string> ExternalSpotRefsOfMultipartSpotsLinkedToSibling(
            Spot siblingSpot,
            IReadOnlyCollection<Spot> spotsToSearch)
            =>
            siblingSpot.IsMultipartSpot
                ? BreakUtilities
                    .GetLinkedMultipartSpots(siblingSpot, spotsToSearch, false)
                    .Select(s => s.ExternalSpotRef)
                    .ToList()
                : new List<string>();
    }
}
