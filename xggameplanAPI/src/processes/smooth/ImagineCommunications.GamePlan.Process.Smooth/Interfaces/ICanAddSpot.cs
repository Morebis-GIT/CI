using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Services;

namespace ImagineCommunications.GamePlan.Process.Smooth.Interfaces
{
    /// <summary>
    /// Provide methods that can determine if a Spot can be added to a Break.
    /// </summary>
    public interface ICanAddSpot
    {
        bool CanAddMultipartSpotAtRequestedPosition(
            string multipartSpot,
            string multipartSpotPosition,
            SpotPositionRules requestedPositionInBreakRule);

        bool CanAddSpotAtBreakPosition(
            int breakPosition,
            SpotPositionRules breakPositionRules);

        bool CanAddSpotAtRequestedPosition(
            string requestedPositionInBreak,
            SpotPositionRules requestedPositionInBreakRule);

        bool CanAddSpotsWithCampaignClashRule(
            IReadOnlyCollection<Spot> spots,
            bool respectCampaignClash,
            ICampaignClashChecker campaignClashChecker
            );

        bool CanAddSpotsWithProductClashRule(
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IImmutableDictionary<string, Clash> clashesByExternalRef,
            ProductClashRules productClashRule,
            bool respectClashExceptions,
            IProductClashChecker productClashChecker,
            ClashExceptionChecker clashExceptionChecker,
            IClashExposureCountService clashExposureCount
            );

        bool CanAddSpotsWithRestrictionRule(
            SmoothProgramme smoothProg,
            IReadOnlyCollection<Spot> spots,
            bool respectRestrictions,
            RestrictionChecker restrictionChecker,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes);

        bool CanAddSpotWithBreakRequest(
            string breakRequest,
            int progBreakCount,
            SpotPositionRules breakPositionRules);

        bool CanAddSpotWithBreakType(string breakType);

        bool CanAddSpotWithTime(DateTime spotStartDateTime, DateTime spotEndDateTime);

        /// <summary>
        /// Whether the spot meets basic eligibility rules for adding to a Break.
        /// </summary>
        bool IsSpotEligibleWhenRespectingSpotTime(Spot spot);

        /// <summary>
        /// Whether the spot meets basic eligibility rules for adding to a Break.
        /// </summary>
        bool IsSpotEligibleWhenIgnoringSpotTime(Spot spot);
    }
}
