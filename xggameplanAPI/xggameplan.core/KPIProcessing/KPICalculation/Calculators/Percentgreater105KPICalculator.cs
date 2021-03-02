using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class PercentGreater105KPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public PercentGreater105KPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var kpiValue = 0;

            foreach (var campaign in _calculationContext.SmoothCampaigns.Value)
            {
                var percentage = KPICalculationHelpers.GetSmoothCampaignRatingPercentage(campaign, _calculationContext.CampaignMetrics.Value);
                if (percentage > 1.05m)
                {
                    kpiValue++;
                }
            }

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.PercentGreater105,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
