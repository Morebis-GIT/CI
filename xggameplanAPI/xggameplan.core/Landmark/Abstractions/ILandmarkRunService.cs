using System;
using System.Threading.Tasks;
using xggameplan.model.Internal.Landmark;
using xggameplan.Model;

namespace xggameplan.core.Landmark.Abstractions
{
    public interface ILandmarkRunService
    {
        /// <summary>
        /// Manages Landmark automated book run triggering process
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<LandmarkTriggerRunResult> TriggerRunAsync(LandmarkRunTriggerModel command, ScheduledRunSettingsModel scheduledRunSettings = null);

        /// <summary>
        /// Gets the Landmark run status.
        /// </summary>
        Task UpdateRunStatusesAsync();

        /// <summary>
        /// Manages Landmark automated book cancel run process
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        Task<CancelRunResult> CancelRunAsync(Guid scenarioId);

        /// <summary>
        /// Checks landmark availability.
        /// </summary>
        /// <returns></returns>
        Task ProbeLandmarkAvailabilityAsync();
    }
}
