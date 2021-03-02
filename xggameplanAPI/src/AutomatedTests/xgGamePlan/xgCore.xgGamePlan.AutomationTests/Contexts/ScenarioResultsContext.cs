using System.Collections.Generic;
using xgCore.xgGamePlan.ApiEndPoints.Models.Runs;
using xgCore.xgGamePlan.ApiEndPoints.Models.ScenarioResults;

namespace xgCore.xgGamePlan.AutomationTests.Contexts
{
    public class ScenarioResultsContext
    {
        public TestEnvironmentRunResult RunResultData { get; set; }

        public ScenarioResultModel ScenarioResult { get; set; }

        public IEnumerable<TopFailureModel> TopFailures { get; set; }

        public IEnumerable<FailureModel> Failures { get; set; }

        public ScenarioResult Metrics { get; set; }

        public IEnumerable<GroupResult> GroupResult { get; set; }

        public IEnumerable<RecommendationSimpleModel> RecommendationsForScenarios { get; set; }

        public IEnumerable<RecommendationAggregateModel> AggregatedReccomendations { get; set; }
        
        public IEnumerable<string> ReturnedOutputFiles { get; set; }
    }
}
