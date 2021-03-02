using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using xggameplan.core.Landmark.Abstractions;
using xggameplan.model.Internal.Landmark;
using xggameplan.Model;

namespace xggameplan.core.Landmark
{
    /// <summary>
    /// Implementation of ILandmarkApiClient interface for local testing purpose
    /// </summary>
    public class LandmarkApiClientStub : ILandmarkApiClient
    {
        private const string FILE_NAME = "kpi.out";
        private const string PAYLOAD = "6,0,1,1,0,0,0,2,0,0,2,1,3,2,1,6,8,16,25,40,207,18,7,15,84.65,2465,10469430950.00,47347755,97.68,89.57";

        public Task<LandmarkBookingResponseModel> TriggerRunAsync(LandmarkBookingRequest bookingRequest, ScheduledRunSettingsModel scheduledRunModel = null)
        {
            return Task.FromResult(new LandmarkBookingResponseModel
            {
                ProcessingId = Guid.NewGuid()
            });
        }

        public Task<LandmarkJobStatusModel> GetRunStatusAsync(Guid processingId)
        {
            return Task.FromResult(new LandmarkJobStatusModel
            {
                JobStatus = ExternalScenarioStatus.Completed,
                ErrorMessage = "Success",
                OutputFiles = new List<LandmarkOutputFileModel>
                {
                    {
                        new LandmarkOutputFileModel
                        {
                            FileName = FILE_NAME,
                            Payload = PAYLOAD
                        }
                    }
                }
            });
        }

        public Task<bool> CancelRunAsync(Guid processingId) => Task.FromResult(false);

        public Task<bool> ProbeLandmarkAsync() => Task.FromResult(true);

        Task<List<LandmarkJobInfo>> ILandmarkApiClient.GetAllJobsAsync() => Task.FromResult(new List<LandmarkJobInfo>(0));
    }
}
