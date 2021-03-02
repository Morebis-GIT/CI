using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class TotalSpotCampaignSpotsKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public TotalSpotCampaignSpotsKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var spotCampaigns = _calculationContext.ActiveCampaigns.Value
                .Where(c => c.DeliveryType == CampaignDeliveryType.Spot)
                .ToArray();

            var spotCampaignsDayParts = KPICalculationHelpers.GetCampaignsDayParts(spotCampaigns);

            var kpiValue = spotCampaignsDayParts.Sum(d => d.TotalSpotCount);

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.TotalSpotCampaignSpots,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
