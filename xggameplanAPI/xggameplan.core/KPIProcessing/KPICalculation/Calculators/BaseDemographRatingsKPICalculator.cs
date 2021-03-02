using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class BaseDemographRatingsKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public BaseDemographRatingsKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var spotCampaigns = _calculationContext.ActiveCampaigns.Value
                .Where(c => c.DeliveryType == CampaignDeliveryType.Spot)
                .ToArray();

            var ratingCampaigns = _calculationContext.ActiveCampaigns.Value
                .Where(c => c.DeliveryType == CampaignDeliveryType.Rating)
                .ToArray();

            var spotCampaignsDayParts = KPICalculationHelpers.GetCampaignsDayParts(spotCampaigns);

            var ratingCampaignsDayParts = KPICalculationHelpers.GetCampaignsDayParts(ratingCampaigns);

            var kpiValue = ratingCampaignsDayParts.Sum(d => d.BaseDemographRatings)
                           + spotCampaignsDayParts.Sum(d => d.BaseDemographRatings);

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.BaseDemographicRatings,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
