using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class TotalPaybackKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public TotalPaybackKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var kpiValue = _calculationContext.ActiveCampaigns.Value
                .Where(c => c.CampaignPaybacks != null)
                .SelectMany(c => c.CampaignPaybacks)
                .Sum(p => p.Amount);

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.TotalPayback,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
