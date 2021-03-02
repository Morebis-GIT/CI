using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.ScenarioResults
{
    public class RecommendationSimpleModel
    {
        public Guid ScenarioId { get; set; }
        public string ExternalSpotRef { get; set; }
        public string Demographic { get; set; }
        public string SalesArea { get; set; }
        public string Processor { get; set; }
        public double SpotEfficiency { get; set; }
    }
}
