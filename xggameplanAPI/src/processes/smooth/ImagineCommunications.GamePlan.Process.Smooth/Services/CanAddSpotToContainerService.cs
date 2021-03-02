using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public class CanAddSpotToContainerService : CanAddSpotService
    {
        public CanAddSpotToContainerService(SmoothBreak smoothBreak) : base(smoothBreak)
        {
        }

        /// <inheritdoc/>
        public override bool CanAddSpotWithBreakRequest(
            string spotPositionRequest,
            IReadOnlyList<SmoothBreak> programmeBreaks,
            SpotPositionRules breakPositionRules)
        {
            if (String.IsNullOrWhiteSpace(spotPositionRequest))
            {
                return true;
            }

            if (ContainerReference.TryParse(
                _smoothBreak.TheBreak.ExternalBreakRef,
                out ContainerReference breakContainerReference))
            {
                if (breakContainerReference.Equals(spotPositionRequest))
                {
                    return true;
                }
            }

            if (!Int32.TryParse(spotPositionRequest, out int requestedContainerNumber))
            {
                return false;
            }

            // Note: this should never happen but it's better to check to stop
            // index out of range exceptions.
            if (programmeBreaks.Count == 0)
            {
                return false;
            }

            // All smooth breaks have a link back to their smooth programme, so
            // just use the first one.
            var breakContainer = programmeBreaks[0].SmoothProgramme.BreakContainers;

            requestedContainerNumber = GetActualBreakPositionFromRelativePosition(
                requestedContainerNumber,
                breakContainer.Count);

            return CanAddSpotAtBreakPosition(
                requestedContainerNumber,
                breakContainerReference.ContainerNumber,
                breakPositionRules);
        }

        public override bool CanAddSpotWithBreakRequestForAtomicSpotWithDefaultPosition(
            string spotPositionRequest,
            IReadOnlyList<SmoothBreak> programmeBreaks,
            SpotPositionRules breakPositionRules)
        {
            return CanAddSpotWithBreakRequest(
                spotPositionRequest,
                programmeBreaks,
                breakPositionRules);
        }

        public override bool CanAddSpotWithBreakRequestForMultipartSpotWithDefaultPosition(
            string spotPositionRequest,
            IReadOnlyList<SmoothBreak> programmeBreaks,
            SpotPositionRules breakPositionRules)
        {
            return CanAddSpotWithBreakRequest(
                spotPositionRequest,
                programmeBreaks,
                breakPositionRules);
        }
    }
}
