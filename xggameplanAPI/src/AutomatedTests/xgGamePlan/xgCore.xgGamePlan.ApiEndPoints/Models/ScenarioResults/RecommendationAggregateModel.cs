using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.ScenarioResults
{
    public class RecommendationAggregateModel
    {
        public string ExternalCampaignNumber { get; set; }
        public double SpotRating { get; set; }
        public string CampaignGroup { get; set; }
        public string CampaignName { get; set; }
        public decimal? TargetRatings { get; set; }
        public decimal? ActualRatings { get; set; }
        public DateTime? EndDateTime { get; set; }
        public bool IsPercentage { get; set; } = false;
        public string AdvertiserName { get; set; }
    }
}
