using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class TotalRatingCampaignSpotsKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public TotalRatingCampaignSpotsKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var ratingsCampaigns = _calculationContext.ActiveCampaigns.Value
                .Where(c => c.DeliveryType == CampaignDeliveryType.Rating)
                .ToArray();

            var ratingCampaignsDayParts = KPICalculationHelpers.GetCampaignsDayParts(ratingsCampaigns);

            var kpiValue = ratingCampaignsDayParts.Sum(d => d.TotalSpotCount);

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.TotalRatingCampaignSpots,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
