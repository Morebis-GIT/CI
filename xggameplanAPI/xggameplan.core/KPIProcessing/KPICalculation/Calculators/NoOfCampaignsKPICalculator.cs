using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class NoOfCampaignsKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public NoOfCampaignsKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var campaignsForScenario = _calculationContext.Recommendations
                .Where(x => x.Action != KPICalculationHelpers.SpotTags.Smoothed)
                .Select(_ => _.ExternalCampaignNumber)
                .Distinct()
                .ToArray();

            var combined = _calculationContext.FailureCampaignExternalIds is null
                ? campaignsForScenario
                : campaignsForScenario.Union(_calculationContext.FailureCampaignExternalIds);

            var kpiValue = combined.Count();

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.NoOfCampaigns,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
