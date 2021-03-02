using System;
using ImagineCommunications.GamePlan.Domain.Scenarios;

namespace xggameplan.model.External
{
    public class TriggeredLandmarkRunInfo
    {
        public string QueueName { get; set; }
        public string RunName { get; set; }
        public string RunType { get; set; }
        public Guid ScenarioId { get; set; }
        public string ScenarioName { get; set; }
        public string DaySelected { get; set; }
        public DateTime? ScheduledDateTime { get; set; }
        public int? Priority { get; set; }
        public ExternalScenarioStatus LandmarkStatus { get; set; }
        public string Comment { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public Guid RunId { get; set; }
        public Guid ExternalRunId { get; set; }
    }
}
