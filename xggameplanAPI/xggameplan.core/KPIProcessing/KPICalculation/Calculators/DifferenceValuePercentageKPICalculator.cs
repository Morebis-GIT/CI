using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class DifferenceValuePercentageKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public DifferenceValuePercentageKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override HashSet<string> DependenciesDefinition { get; } = new HashSet<string>
        {
            ScenarioKPINames.DifferenceValue
        };

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var differenceValue = GetDependencyValue(ScenarioKPINames.DifferenceValue).Value;
            var campaignsRevenue = _calculationContext.ActiveCampaigns.Value.Sum(c => c.RevenueBudget);

            var kpiValue = campaignsRevenue == 0
                ? 0
                : Math.Round(differenceValue / campaignsRevenue * 100, 2);

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.DifferenceValuePercentage,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
