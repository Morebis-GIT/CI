using System;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Views.Tenant
{
    public class RecommendationAggregate
    {
        public string ExternalCampaignNumber { get; set; }
        public decimal SpotRating { get; set; }
        public string CampaignGroup { get; set; }
        public string CampaignName { get; set; }
        public decimal? ActualRatings { get; set; }
        public decimal? TargetRatings { get; set; }
        public DateTime? EndDateTime { get; set; }
        public bool IsPercentage { get; set; } = false;
        public string AdvertiserName { get; set; }
    }
}
