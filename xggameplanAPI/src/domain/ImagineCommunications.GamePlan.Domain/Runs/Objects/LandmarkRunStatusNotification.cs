using System;

namespace ImagineCommunications.GamePlan.Domain.Runs.Objects
{
    public class LandmarkRunStatusNotification
    {
        public Guid runId { get; set; }
        public Guid externalRunId { get; set; }
        public string status { get; set; }
        public DateTime date { get; set; }
        public TimeSpan time { get; set; }
        public string errorMessage { get; set; }
    }
}
