using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    /// <summary>
    /// Result of validating adding spot to break.
    /// </summary>
    public class SmoothFailuresForSpotCollection
    {
        public SmoothFailuresForSpotCollection(Guid spotId) =>
            SpotId = spotId;

        public Guid SpotId { get; }

        public List<SmoothFailureAndReasonForFailure> Failures { get; } =
            new List<SmoothFailureAndReasonForFailure>();
    }
}
