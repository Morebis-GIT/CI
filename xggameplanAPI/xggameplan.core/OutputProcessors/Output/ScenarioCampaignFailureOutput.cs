using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;

namespace xggameplan.OutputFiles.Processing
{
    public class ScenarioCampaignFailureOutput
    {
        public Guid ScenarioId { get; set; }
        public List<ScenarioCampaignFailure> Data { get; set; }
    }
}
