using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics.Objects;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation
{
    public class ScenarioCampaignKPIsCalculator
    {
        public ScenarioCampaignMetricItem CalculateCampaignKPIs(CampaignReducedModel campaign, IEnumerable<Recommendation> campaignRecommendations, decimal nominalPrice)
        {
            int totalSpotsBooked = 0;
            int zeroRatedSpots = 0;
            double nominalValue = 0d;

            foreach (var campaignRecommendation in campaignRecommendations)
            {
                nominalValue += campaignRecommendation.NominalPrice;

                if (campaignRecommendation.Action != KPICalculationHelpers.SpotTags.Booked)
                {
                    continue;
                }

                totalSpotsBooked++;

                if (campaignRecommendation.SpotRating == 0)
                {
                    zeroRatedSpots++;
                }
            }

            var campaignRevenue = campaign.RevenueBudget;
            var totalNominalValue = nominalValue + (double)nominalPrice;
            var differenceValueDelivered = totalNominalValue - campaignRevenue;
            var differenceValueDeliveredPercentage = campaignRevenue == 0 ? 0 : Math.Round(differenceValueDelivered / campaignRevenue, 2);

            return new ScenarioCampaignMetricItem
            {
                CampaignExternalId = campaign.ExternalId,
                TotalSpots = totalSpotsBooked,
                ZeroRatedSpots = zeroRatedSpots,
                NominalValue = nominalValue,
                TotalNominalValue = totalNominalValue,
                DifferenceValueDelivered = differenceValueDelivered,
                DifferenceValueDeliveredPercentage = differenceValueDeliveredPercentage
            };
        }
    }
}
