using System;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;

namespace xggameplan.AutoBooks.Abstractions
{
    /// <summary>
    /// Interface for handling output data for run scenario
    /// </summary>
    public interface IAutoBookOutputHandler
    {
        /// <summary>
        /// Hands downloading and processing output data
        /// </summary>
        /// <param name="run"></param>
        /// <param name="scenarioId"></param>
        void Handle(Run run, Guid scenarioId);

        /// <summary>
        /// Performs cleanup of output data
        /// </summary>
        /// <param name="run"></param>
        /// <param name="scenarioId"></param>
        void CleanUp(Run run, Guid scenarioId);
    }
}
