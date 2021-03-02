using System;
using ImagineCommunications.GamePlan.Domain.Scenarios;

namespace ImagineCommunications.GamePlan.Domain.Runs.Objects
{
    public class ExternalRunInfo
    {
        public int RunScenarioId { get; set; }

        public Guid ExternalRunId { get; set; }
        public ExternalScenarioStatus ExternalStatus { get; set; }
        public DateTime ExternalStatusModifiedDate { get; set; }

        public string QueueName { get; set; }
        public int? Priority { get; set; }
        public DateTime? ScheduledDateTime { get; set; }
        public string Comment { get; set; }

        public int? CreatorId { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
