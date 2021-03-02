using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers
{
    public class CampaignReducedModel_Transformer : AbstractTransformerCreationTask<Campaign>
    {
        public CampaignReducedModel_Transformer()
        {
            TransformResults = campaigns =>
                from campaign in campaigns
                let campaignId = campaign.Id.ToString()
                select new
                {
                    Id = Guid.Parse(campaignId.Substring(campaignId.IndexOf('/') + 1)),
                    campaign.CustomId,
                    campaign.ExternalId,
                    campaign.Name,
                    campaign.DemoGraphic,
                    campaign.StartDateTime,
                    campaign.EndDateTime,
                    campaign.Product,
                    campaign.RevenueBudget,
                    campaign.TargetRatings,
                    campaign.ActualRatings,
                    campaign.CampaignGroup,
                    campaign.IsPercentage,
                    campaign.Status,
                    campaign.BusinessType,
                    campaign.DeliveryType,
                    campaign.IncludeOptimisation,
                    campaign.TargetZeroRatedBreaks,
                    campaign.InefficientSpotRemoval,
                    campaign.ExpectedClearanceCode,
                    campaign.CampaignPassPriority,
                    campaign.CampaignSpotMaxRatings,
                    IsActive = (campaign.DeliveryType == CampaignDeliveryType.Spot
                                   ? campaign.TargetRatings >= default(decimal)
                                   : campaign.TargetZeroRatedBreaks || campaign.TargetRatings > default(decimal)) &&
                               campaign.SalesAreaCampaignTarget != null &&
                               campaign.SalesAreaCampaignTarget.Any() &&
                               !campaign.Status.Equals("C", StringComparison.OrdinalIgnoreCase)
                };
        }
    }
}
