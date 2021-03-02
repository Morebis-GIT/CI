using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class RatingCampaignsRatedSpotsKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public RatingCampaignsRatedSpotsKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var totalRatedSpots = _calculationContext.Recommendations
                .Where(x => x.SpotRating > 0 && x.Action == KPICalculationHelpers.SpotTags.Booked);

            var ratingCampaignIds = new HashSet<string>(_calculationContext.ActiveCampaigns.Value
                .Where(c => c.DeliveryType == CampaignDeliveryType.Rating).Select(c => c.ExternalId));

            var kpiValue = totalRatedSpots.Count(s => ratingCampaignIds.Contains(s.ExternalCampaignNumber));

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.RatingCampaignsRatedSpots,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
