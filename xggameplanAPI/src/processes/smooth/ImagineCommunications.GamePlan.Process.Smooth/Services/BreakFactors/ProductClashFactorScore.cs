using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    /// <summary>
    /// Calculates the factor score for product clashes at both child and parent level.
    /// </summary>
    public class ProductClashFactorScore
        : IProductClashFactorScore
    {
        private readonly IClashExposureCountService _effectiveClashExposureCount;

        public ProductClashFactorScore(IClashExposureCountService effectiveClashExposureCount)
        {
            _effectiveClashExposureCount = effectiveClashExposureCount;
        }

        /// <summary>
        /// Calculates the factor score for product clashes at both child and
        /// parent level
        /// </summary>
        /// <param name="spotsInTheBreak">
        /// A collection of spots currently placed in a break.
        /// </param>
        /// <param name="spotsToPlace">
        /// A collection of spots to consider placing in the break.
        /// </param>
        /// <param name="spotInfos">
        /// Additional information about the spots to place.
        /// </param>
        /// <param name="productsByExternalRef">
        /// A collection of products that may clash with the spots to place.
        /// </param>
        /// <param name="clashesByExternalRef">
        /// A collection of possible clashes for each of the products.
        /// </param>
        /// <param name="productClashChecker">
        /// A function to determine which spots to place clash with each other.
        /// </param>
        public double GetFactorScoreForProductClashes(
            IReadOnlyCollection<Spot> spotsInTheBreak,
            IReadOnlyCollection<Spot> spotsToPlace,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Product> productsByExternalRef,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            Func<Spot, IReadOnlyCollection<Spot>, IReadOnlyDictionary<Guid, SpotInfo>, ClashCodeLevel, IReadOnlyCollection<Spot>> productClashChecker
            )
        {
            double factorScore = 0;

            // Calculate score based on both child & parent clashes
            const double sameChildClashWeight = 3;
            const double sameParentClashWeight = 1;

            if (spotsToPlace is null)
            {
                return 0d;
            }

            foreach (var spot in spotsToPlace)
            {
                if (spot.Product is null)
                {
                    continue;
                }

                if (!productsByExternalRef.TryGetValue(spot.Product, out Product product))
                {
                    continue;
                }

                if (product is null)
                {
                    continue;
                }

                if (!clashesByExternalRef.TryGetValue(product.ClashCode, out Clash childClash))
                {
                    continue;
                }

                if (childClash is null)
                {
                    continue;
                }

                // Calculate score for child
                int productClashCountChild = productClashChecker(
                    spot,
                    spotsInTheBreak,
                    spotInfos,
                    ClashCodeLevel.Child
                    ).Count;

                // Calculate score for parent
                int productClashCountParent = productClashChecker(
                    spot,
                    spotsInTheBreak,
                    spotInfos,
                    ClashCodeLevel.Parent
                    ).Count;

                double parentClashScore = 0;
                if (HasParentClash(clashesByExternalRef, childClash, out Clash parentClash))
                {
                    parentClashScore = ExposureClashScore(
                        spot,
                        parentClash,
                        productClashCountParent,
                        sameParentClashWeight
                        );
                }

                double childClashScore = ExposureClashScore(spot, childClash, productClashCountChild, sameChildClashWeight);

                factorScore += childClashScore + parentClashScore;
            }

            return factorScore;
        }

        private static bool HasParentClash(
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            Clash childClash,
            out Clash parentClash)
        {
            parentClash = null;

            return childClash.ParentExternalidentifier != null
                && clashesByExternalRef.TryGetValue(childClash.ParentExternalidentifier, out parentClash)
                && parentClash != null;
        }

        private double ExposureClashScore(
            Spot spot,
            Clash clash,
            int productClashCount,
            double weight
            )
        {
            double effectiveExposureCount = _effectiveClashExposureCount.Calculate(
                clash.Differences,
                (clash.DefaultPeakExposureCount, clash.DefaultOffPeakExposureCount),
                (spot.StartDateTime, spot.SalesArea));

            return 1 / effectiveExposureCount * productClashCount * weight;
        }
    }
}
