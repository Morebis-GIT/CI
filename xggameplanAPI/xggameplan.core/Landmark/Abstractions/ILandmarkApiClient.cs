using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using xggameplan.model.Internal.Landmark;
using xggameplan.Model;

namespace xggameplan.core.Landmark.Abstractions
{
    /// <summary>
    /// Interface for communication with Landmark run API.
    /// </summary>
    public interface ILandmarkApiClient
    {
        /// <summary>
        /// Requests an automated book run in Landmark.
        /// </summary>
        /// <param name="bookingRequest"></param>
        Task<LandmarkBookingResponseModel> TriggerRunAsync(LandmarkBookingRequest bookingRequest, ScheduledRunSettingsModel scheduledRunModel = null);

        /// <summary>
        /// Gets all jobs from the Landmark.
        /// </summary>
        /// <returns></returns>
        Task<List<LandmarkJobInfo>> GetAllJobsAsync();

        /// <summary>
        /// Checks the status of a submitted automated booking run.
        /// </summary>
        /// <param name="processingId">Landmark run job identifier</param>
        Task<LandmarkJobStatusModel> GetRunStatusAsync(Guid processingId);

        /// <summary>
        /// Cancels run on Landmark side.
        /// </summary>
        /// <param name="processingId">Landmark run job identifier.</param>
        /// <returns></returns>
        Task<bool> CancelRunAsync(Guid processingId);

        /// <summary>
        /// Checks connection with landmark.
        /// </summary>
        /// <returns><c>true</c> if landmark is available; otherwise, <c>false</c></returns>
        Task<bool> ProbeLandmarkAsync();
    }
}
