using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Campaigns.Projections;

namespace ImagineCommunications.GamePlan.Domain.Campaigns.Objects
{
    public class CampaignReducedModel : IReducedCampaign
    {
        public Guid Id { get; set; }
        public int CustomId { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string DemoGraphic { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Product { get; set; }
        public double RevenueBudget { get; set; }
        public decimal TargetRatings { get; set; }
        public decimal ActualRatings { get; set; }
        public string CampaignGroup { get; set; }
        public bool IsPercentage { get; set; }
        public string Status { get; set; }
        public string BusinessType { get; set; }
        public CampaignDeliveryType DeliveryType { get; set; }
        public bool IncludeOptimisation { get; set; }
        public bool TargetZeroRatedBreaks { get; set; }
        public bool InefficientSpotRemoval { get; set; }
        public string ExpectedClearanceCode { get; set; }
        public int CampaignPassPriority { get; set; }
        public int CampaignSpotMaxRatings { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// Indexes list by CustomId
        /// </summary>
        /// <param name="campaigns"></param>
        /// <returns></returns>
        public static Dictionary<int, CampaignReducedModel> IndexListByCustomId(IEnumerable<CampaignReducedModel> campaigns)
        {
            var campaignsIndexed = new Dictionary<int, CampaignReducedModel>();

            foreach (var campaign in campaigns)
            {
                if (!campaignsIndexed.ContainsKey(campaign.CustomId))
                {
                    campaignsIndexed.Add(campaign.CustomId, campaign);
                }
            }

            return campaignsIndexed;
        }
    }
}
