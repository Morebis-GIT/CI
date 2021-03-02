using System;

namespace ImagineCommunications.GamePlan.Domain.Recommendations.Objects
{
    public class RecommendationsByScenarioReduceResult
    {
        public Guid ScenarioId { get; set; }
        public string ExternalCampaignNumber { get; set; }
        public string Action { get; set; }
        public decimal SpotRating { get; set; }

        public int Count { get; set; }
    }
}
