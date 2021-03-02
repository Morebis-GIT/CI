using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Interfaces
{
    /// <summary>
    /// Smooth diagnostics logging interface
    /// </summary>
    public interface ISmoothDiagnostics
    {
        /// <summary>
        /// Logs places spots
        /// </summary>
        /// <param name="prog"></param>
        /// <param name="smoothBreaks"></param>
        void LogPlacedSmoothSpots(
            Programme prog,
            IReadOnlyCollection<SmoothBreak> smoothBreaks);

        /// <summary>
        /// Logs unplaced spots
        /// </summary>
        /// <param name="spots"></param>
        void LogUnplacedSmoothSpots(IReadOnlyCollection<Spot> spots);

        /// <summary>
        /// Enabled logging details information for debug
        /// </summary>
        bool LogDetail { get; set; }

        /// <summary>
        /// Logs programme, for diagnostics
        /// </summary>
        /// <param name="smoothProgId"></param>
        /// <param name="progSmoothBreaks"></param>
        /// <param name="countSpotsUnplacedBeforeSmooth"></param>
        /// <param name="countSpotsUnplacedAfterSmooth"></param>
        void LogProgramme(
            Guid smoothProgId,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            int countSpotsUnplacedBeforeSmooth,
            int countSpotsUnplacedAfterSmooth);

        /// <summary>
        /// Logs best break factor message
        /// </summary>
        /// <param name="message"></param>
        void LogBestBreakFactorMessage(
            SmoothPass smoothPass,
            SmoothPassDefaultIteration smoothPassIteration,
            BestBreakFactorGroup bestBreakFactorGroup,
            Break oneBreak,
            IReadOnlyCollection<Spot> spots,
            decimal? bestBreakScore,
            string message);

        /// <summary>
        /// Logs an action associated with a spot
        /// </summary>
        /// <param name="smoothPass"></param>
        /// <param name="smoothSpot"></param>
        /// <param name="smoothBreak"></param>
        /// <param name="action"></param>
        /// <param name="message"></param>
        void LogSpotAction(
            SmoothPass smoothPass,
            int smoothPassIteration,
            Spot spot,
            SmoothBreak smoothBreak,
            SmoothSpot.SmoothSpotActions action,
            string message);

        /// <summary>
        /// Flushes queued changes
        /// </summary>
        void Flush();
    }
}
