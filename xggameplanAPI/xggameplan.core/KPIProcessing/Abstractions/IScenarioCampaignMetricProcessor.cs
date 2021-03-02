using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;

namespace xggameplan.KPIProcessing.Abstractions
{
    /// <summary>
    /// Functionality for generating scenario campaign metrics
    /// </summary>
    public interface IScenarioCampaignMetricsProcessor
    {
        /// <summary>
        /// Generates the scenario campaign metrics.
        /// </summary>
        /// <param name="runId">Run identifier.</param>
        /// <param name="scenarioId">Scenario identifier.</param>
        void ProcessScenarioCampaignMetrics(Guid runId, Guid scenarioId, IEnumerable<Recommendation> scenarioRecommendations);
    }
}
