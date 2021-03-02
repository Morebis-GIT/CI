using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects
{
    /// <summary>
    /// Model for searching scenario campaign failures
    /// </summary>
    public class ScenarioCampaignFailureSearchQueryModel : BaseQueryModel
    {
        public Guid ScenarioId { get; set; }
        public IEnumerable<string> ExternalCampaignIds { get; set; }
        public IEnumerable<string> SalesAreaGroupNames { get; set; }
        public IEnumerable<ScenarioCampaignFailureSearchQueryStrikeWeightModel> StrikeWeights { get; set; }
    }
}
