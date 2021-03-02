using System;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;

namespace xggameplan.AutoBooks.Abstractions
{
    /// <summary>
    /// Interface for handling input data for run scenario
    /// </summary>
    public interface IAutoBookInputHandler
    {
        /// <summary>
        /// Handles uploading of input data
        /// </summary>
        /// <param name="run"></param>
        /// <param name="scenarioId"></param>
        void Handle(Run run, Guid scenarioId);

        /// <summary>
        /// Performs cleanup of input data
        /// </summary>
        /// <param name="run"></param>
        /// <param name="scenarioId"></param>
        void CleanUp(Run run, Guid scenarioId);
    }
}
