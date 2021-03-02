using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using Quartz;
using xggameplan.core.Exceptions;
using xggameplan.core.FeatureManagement.Attributes;
using xggameplan.core.Landmark.Abstractions;

namespace xggameplan.SchedulerTasks.Jobs
{
    [ScopeJob(JobScopeType.Tenant)]
    [FeatureFilter(nameof(ProductFeature.LandmarkBooking))]
    public class RunStatusChecker : IJob
    {
        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly ILandmarkRunService _landmarkRunService;

        public RunStatusChecker(ILandmarkRunService landmarkRunService) => _landmarkRunService = landmarkRunService;

        public async Task Execute(IJobExecutionContext context)
        {
            if (!await _semaphoreSlim.WaitAsync(0).ConfigureAwait(false))
            {
                return;
            }

            try
            {
                await _landmarkRunService.ProbeLandmarkAvailabilityAsync().ConfigureAwait(false);
                await _landmarkRunService.UpdateRunStatusesAsync().ConfigureAwait(false);
            }
            catch (LandmarkNotAvailableException)
            {
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
