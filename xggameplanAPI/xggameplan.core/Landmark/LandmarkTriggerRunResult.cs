using System;
using ImagineCommunications.GamePlan.Domain.Scenarios;

namespace xggameplan.core.Landmark
{
    public class LandmarkTriggerRunResult
    {
        public Guid ExternalRunId { get; set; }
        public Guid RunId { get; set; }
        public ExternalScenarioStatus Status { get; set; }
    }
}
