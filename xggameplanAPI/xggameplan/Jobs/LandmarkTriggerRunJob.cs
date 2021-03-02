using System;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using NodaTime;
using xggameplan.common.BackgroundJobs;
using xggameplan.core.Hubs;
using xggameplan.core.Landmark.Abstractions;
using xggameplan.model.Internal.Landmark;
using xggameplan.Model;

namespace xggameplan.Jobs
{
    public class LandmarkTriggerRunJob : IBackgroundJob
    {
        private readonly ILandmarkRunService _landmarkRunService;
        private readonly IHubNotification<LandmarkRunStatusNotification> _runStatusChangedNotifier;
        private readonly IClock _clock;

        public LandmarkTriggerRunJob(
            ILandmarkRunService landmarkRunService,
            IHubNotification<LandmarkRunStatusNotification> runStatusChangedNotifier,
            IClock clock)
        {
            _landmarkRunService = landmarkRunService;
            _runStatusChangedNotifier = runStatusChangedNotifier;
            _clock = clock;
        }

        public async Task Execute(LandmarkRunTriggerModel command,
            ScheduledRunSettingsModel scheduledRunSettings = null)
        {
            var notification = new LandmarkRunStatusNotification();

            try
            {
                var res = await _landmarkRunService.TriggerRunAsync(command, scheduledRunSettings)
                    .ConfigureAwait(false);

                notification.runId = res.RunId;
                notification.externalRunId = res.ExternalRunId;
                notification.status = res.Status.ToString();
            }
            catch (Exception ex)
            {
                notification.errorMessage = ex.Message;
            }
            finally
            {
                var date = _clock.GetCurrentInstant().ToDateTimeUtc();
                notification.date = date;
                notification.time = date.TimeOfDay;

                _runStatusChangedNotifier.Notify(notification);
            }
        }
    }
}
