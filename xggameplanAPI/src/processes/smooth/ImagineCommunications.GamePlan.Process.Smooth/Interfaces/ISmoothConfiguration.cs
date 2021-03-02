using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Interfaces
{
    /// <summary>
    /// Interface to Smooth configuration. This is necessary because some of the configuration
    /// contains code rather than data and so we can't just store the configuration in the database.
    /// </summary>
    public interface ISmoothConfiguration
    {
        /// <summary>
        /// Version
        /// </summary>
        string Version { get; }

        bool RestrictionCheckEnabled { get; }

        bool ClashExceptionCheckEnabled { get; }

        /// <summary>
        /// ExternalCampaignRefs to exclude. E.g. GRID spots
        /// </summary>
        List<string> ExternalCampaignRefsToExclude { get; }

        /// <summary>
        /// Whether to add Recommendation for excluded campaign spots
        /// </summary>
        bool RecommendationsForExcludedCampaigns { get; }

        /// <summary>
        /// Whether to add SmoothFailure for excluded campaign spots
        /// </summary>
        bool SmoothFailuresForExcludedCampaigns { get; }

        /// <summary>
        /// Returns list of Smooth passes in sequence order.
        /// </summary>
        IImmutableList<SmoothPass> SortedSmoothPasses { get; }

        /// <summary>
        /// Returns list of default pass iterations for the spot
        /// </summary>
        /// <param name="spot"></param>
        /// <param name="smoothPass"></param>
        /// <returns></returns>
        List<SmoothPassDefaultIteration> GetSmoothPassDefaultIterations(Spot spot, SmoothPass smoothPass);

        /// <summary>
        /// Returns list of unplaced pass iterations for the spot
        /// </summary>
        /// <param name="spot"></param>
        /// <param name="smoothPass"></param>
        /// <returns></returns>
        List<SmoothPassUnplacedIteration> GetSmoothPassUnplacedIterations(Spot spot, SmoothPass smoothPass);

        /// <summary>
        /// Returns the list of best break factor groups for determining the best break to add the spot(s) to
        /// </summary>
        /// <param name="spots"></param>
        /// <param name="progSmoothBreaks"></param>
        /// <param name="smoothPass"></param>
        /// <param name="smoothPassIteration"></param>
        /// <returns></returns>
        IReadOnlyCollection<BestBreakFactorGroup> GetBestBreakFactorGroupsData(
            IReadOnlyCollection<Spot> spots,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            SmoothPass smoothPass,
            SmoothPassDefaultIteration smoothPassIteration);

        /// <summary>
        /// Sorts spots to be placed
        /// </summary>
        IEnumerable<Spot> SortSpotsToPlace(
            IEnumerable<Spot> spots,
            (DateTime programmeStartDateTime, Duration programmeDuration) programmeTxDetails);

        /// <summary>
        /// Returns spots that can be moved from the break in order to make room for the spot
        /// </summary>
        /// <param name="spotToPlace"></param>
        /// <param name="smoothBreak"></param>
        /// <returns></returns>
        List<SmoothSpot> GetSpotsThatCanBeMoved(Spot spotToPlace, SmoothBreak smoothBreak);

        /// <summary>
        /// Diagnostic configuration
        /// </summary>
        SmoothDiagnosticConfiguration DiagnosticConfiguration { get; }

        /// <summary>
        /// The Smooth configuration document itself.
        /// </summary>
        SmoothConfiguration SmoothConfiguration { get; }
    }
}
