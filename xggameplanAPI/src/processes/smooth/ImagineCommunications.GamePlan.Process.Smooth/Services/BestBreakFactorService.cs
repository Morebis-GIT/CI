using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public class BestBreakFactorService
    {
        private readonly IClashExposureCountService _effectiveClashExposureCount;

        public BestBreakFactorService(
            IClashExposureCountService clashExposureCount) =>
            _effectiveClashExposureCount = clashExposureCount;

        /// <summary>
        /// <para>
        /// Calculates the score for the factor for adding the spot(s) to the
        /// break. We generate a score for the particular factor, scale it so
        /// that the score is in the same range for every factor (0-1000) and
        /// then scale it by the factor priority.
        /// </para>
        /// <para>
        /// When calculating scores the filter factors should be zero/positive
        /// value and other factors should start from one not zero. This is
        /// necessary for other factors because it is possible to configure the
        /// group to evaluate a score of zero if any factor is zero but this is
        /// only intended for filter factors.
        /// </para>
        /// </summary>
        public decimal GetBestBreakFactorScoreForFactor(
            SmoothBreak smoothBreak,
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IReadOnlyDictionary<string, Product> productsByExternalRef,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            IReadOnlyCollection<double> randomScoreByBreak,
            BestBreakFactor bestBreakFactor,
            SmoothResources smoothResources
            )
        {
            var sponsoredSpots = spots.Where(s => s.Sponsored).ToList();

            double standardScore = 0;
            IStandardScore breakFactor = null;

            switch (bestBreakFactor.Factor)
            {
                case BestBreakFactors.BreaksInAscendingPosition:
                    breakFactor = new BreaksInAscendingPositionBreakFactor(
                        progSmoothBreaks,
                        smoothBreak
                        );

                    break;

                case BestBreakFactors.BreaksInDescendingPosition:
                    breakFactor = new BreaksInDescendingPositionBreakFactor(
                        progSmoothBreaks,
                        smoothBreak
                        );

                    break;

                case BestBreakFactors.FewestSpotsInBreak:
                    breakFactor = new FewestSpotsInBreakBreakFactor(
                        smoothBreak
                        );

                    break;

                case BestBreakFactors.IsFillsBreakDuration:
                    breakFactor = new IsFillsBreakDurationBreakFactor(
                        smoothBreak,
                        spots
                        );

                    break;

                case BestBreakFactors.IsSponsoredSpotBeingPlacedWithNoBreakPosition:
                    breakFactor = new IsSponsoredSpotBeingPlacedWithNoBreakPositionBreakFactor(
                        sponsoredSpots
                        );

                    break;

                case BestBreakFactors.IsOtherBreaksHaveSpotsForSameSponsor:
                    breakFactor = new IsOtherBreaksHaveSpotsForSameSponsorBreakFactor(
                        smoothBreak,
                        sponsoredSpots,
                        progSmoothBreaks,
                        spotInfos
                        );

                    break;

                case BestBreakFactors.IsAnyBreaksHaveSpotsForSameSponsor:
                    breakFactor = new IsAnyBreaksHaveSpotsForSameSponsorBreakFactor(
                        sponsoredSpots,
                        progSmoothBreaks,
                        spotInfos
                        );

                    break;

                case BestBreakFactors.IsNoBreaksHaveSpotsForSameSponsor:
                    breakFactor = new IsNoBreaksHaveSpotsForSameSponsorBreakFactor(
                        sponsoredSpots,
                        progSmoothBreaks,
                        spotInfos
                        );

                    break;

                case BestBreakFactors.FewestProductClashes:
                    breakFactor = new FewestProductClashesBreakFactor(
                        smoothBreak,
                        spots,
                        spotInfos,
                        productsByExternalRef,
                        clashesByExternalRef,
                        smoothResources.ProductClashChecker,
                        _effectiveClashExposureCount
                        );

                    break;

                case BestBreakFactors.FewestProductClashesAtChildLevel:
                    breakFactor = new FewestProductClashesAtChildLevelBreakFactor(
                        spots,
                        smoothBreak,
                        spotInfos,
                        smoothResources.ProductClashChecker
                        );

                    break;

                case BestBreakFactors.FewestProductClashesAtParentLevel:
                    breakFactor = new FewestProductClashesAtParentLevelBreakFactor(
                        smoothResources.ProductClashChecker,
                        spots,
                        smoothBreak,
                        spotInfos
                        );

                    break;

                case BestBreakFactors.FewestCampaignClashes:
                    breakFactor = new FewestCampaignClashesBreakFactor(
                        spots,
                        smoothBreak,
                        smoothResources.CampaignClashChecker
                        );

                    break;

                case BestBreakFactors.FewestCampaignAndProductClashes:
                    breakFactor = new FewestCampaignAndProductClashesBreakFactor(
                        smoothBreak,
                        spots,
                        spotInfos,
                        productsByExternalRef,
                        clashesByExternalRef,
                        smoothResources.ProductClashChecker,
                        smoothResources.CampaignClashChecker,
                        _effectiveClashExposureCount);

                    break;

                case BestBreakFactors.IsLeavesDefaultBreakMultipleDurations:
                    breakFactor = new IsLeavesDefaultBreakMultipleDurationsBreakFactor(
                        spots,
                        smoothBreak
                        );

                    break;

                case BestBreakFactors.IsLeaves15SecBreakMultipleDurations:
                    breakFactor = new IsLeaves15SecBreakMultipleDurationsBreakFactor(
                        spots,
                        smoothBreak
                        );

                    break;

                case BestBreakFactors.IsLeavesDefaultBreakMultipleDurationsOrFillsBreak:
                    breakFactor = new IsLeavesDefaultBreakMultipleDurationsOrFillsBreakBreakFactor(
                        spots,
                        smoothBreak
                        );

                    break;

                case BestBreakFactors.LargestRemainingBreakDuration:
                    breakFactor = new LargestRemainingBreakDurationBreakFactor(
                        smoothBreak
                        );

                    break;

                case BestBreakFactors.FewestMatchingSponsorSpots:
                    breakFactor = new FewestMatchingSponsorSpotsBreakFactor(
                        spots,
                        smoothBreak,
                        spotInfos
                        );

                    break;

                case BestBreakFactors.IsNoSponsorSpotsForSameSponsor:
                    breakFactor = new IsNoSponsorSpotsForSameSponsorBreakFactor(
                        smoothBreak,
                        spots,
                        spotInfos
                        );

                    break;

                case BestBreakFactors.IsNoSponsorSpotsForAnySponsor:
                    breakFactor = new IsNoSponsorSpotsForAnySponsorBreakFactor(
                        sponsoredSpots
                        );

                    break;

                case BestBreakFactors.IsFirstBreak:
                    breakFactor = new IsFirstBreakBreakFactor(
                        smoothBreak
                        );

                    break;

                case BestBreakFactors.IsFirstTwoBreaks:
                    breakFactor = new IsFirstTwoBreaksBreakFactor(
                        smoothBreak
                        );

                    break;

                case BestBreakFactors.IsLastBreak:
                    breakFactor = new IsLastBreakBreakFactor(
                        smoothBreak,
                        progSmoothBreaks
                        );

                    break;

                case BestBreakFactors.IsLastTwoBreaks:
                    breakFactor = new IsLastTwoBreaksBreakFactor(
                        smoothBreak,
                        progSmoothBreaks
                        );

                    break;

                case BestBreakFactors.IsFirstHalfOfProgramme:
                    breakFactor = new IsFirstHalfOfProgrammeBreakFactor(
                        smoothBreak,
                        progSmoothBreaks
                        );

                    break;

                case BestBreakFactors.IsLastHalfOfProgramme:
                    breakFactor = new IsLastHalfOfProgrammeBreakFactor(
                        smoothBreak,
                        progSmoothBreaks
                        );

                    break;

                case BestBreakFactors.IsBreakDurationIs30SecMultiples:
                    breakFactor = new IsBreakDurationIs30SecMultiplesBreakFactor(
                        smoothBreak
                        );

                    break;

                case BestBreakFactors.FewestRequestedPositionInBreakRequestsFirstOrLastOnly:
                    breakFactor = new FewestRequestedPositionInBreakRequestsFirstOrLastOnlyBreakFactor(
                        smoothBreak,
                        spots
                        );

                    break;

                case BestBreakFactors.RandomBreak:
                    breakFactor = new RandomBreakBreakFactor(
                        randomScoreByBreak,
                        smoothBreak
                        );

                    break;

                case BestBreakFactors.SpotDurationBalance:
                    breakFactor = new SpotDurationBalanceBreakFactor(
                        spots,
                        smoothBreak
                        );

                    break;

                case BestBreakFactors.SameNonZeroScoreForAllBreaks:
                    breakFactor = new SameNonZeroScoreForAllBreaksBreakFactor();

                    break;

                // This factor is not used. Included here to complete the switch.
                case BestBreakFactors.BreakEfficiency:
                default:
                    break;
            }

            standardScore = Math.Max(breakFactor?.StandardScore ?? 0, 0);

            // Return score that takes into account the factor priority, decimal
            // allows the most digits (i.e. most factors)
            int[] values = new int[BestBreakFactorGroupItem.MaxBreakFactorPriority];
            values[bestBreakFactor.Priority - 1] = (int)standardScore;

            var valuesString = new StringBuilder("0.");

            for (int index = 0; index < values.Length; index++)
            {
                _ = valuesString.Append(Math.Max(0, values[index]).ToString("D4"));
            }

            return Convert.ToDecimal(valuesString.ToString());
        }
    }
}
