using System;

namespace xggameplan.Model
{
    /// <summary>
    /// Recommendation, simple properties only
    /// </summary>
    public class RecommendationAggregateModel
    {
        /// <summary>
        /// External Campaign reference
        /// </summary>
        public string ExternalCampaignNumber { get; set; }

        /// <summary>
        /// Spot Rating
        /// </summary>
        public decimal SpotRating { get; set; }

        /// <summary>
        /// Campaign Group
        /// </summary>
        public string CampaignGroup { get; set; }

        /// <summary>
        /// Campaign Name - First campaign name if its a campaign group
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        /// TargetRatings
        /// </summary>
        public decimal? TargetRatings { get; set; }

        /// <summary>
        /// ActualRatings
        /// </summary>
        public decimal? ActualRatings { get; set; }

        /// <summary>
        /// EndDateTime
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// Is the ratings a value or a percentage?
        /// </summary>
        public bool IsPercentage { get; set; } = false;

        /// <summary>
        /// AdvertiserName
        /// </summary>
        public string AdvertiserName { get; set; }
    }
}
