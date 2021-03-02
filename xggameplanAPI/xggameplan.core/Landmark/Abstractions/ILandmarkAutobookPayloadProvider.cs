using System;
using System.Collections.Generic;
using xggameplan.Model;

namespace xggameplan.core.Landmark.Abstractions
{
    /// <summary>
    /// The payload provider for Landmark AutoBook.
    /// </summary>
    public interface ILandmarkAutoBookPayloadProvider
    {
        /// <summary>
        /// Gets the run input files payload for scenario with the specified Id.
        /// </summary>
        /// <param name="runId">The run Id.</param>
        /// <param name="scenarioId">The scenario Id.</param>
        /// <returns>The run input files payload for scenario with the specified Id.</returns>
        IEnumerable<LandmarkInputFilePayload> GetFiles(Guid runId, Guid scenarioId);
    }
}
