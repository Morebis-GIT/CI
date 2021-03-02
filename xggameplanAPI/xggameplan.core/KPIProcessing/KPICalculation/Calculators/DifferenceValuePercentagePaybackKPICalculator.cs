using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class DifferenceValuePercentagePaybackKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public DifferenceValuePercentagePaybackKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override HashSet<string> DependenciesDefinition { get; } = new HashSet<string>
        {
            ScenarioKPINames.TotalPayback,
            ScenarioKPINames.DifferenceValueWithPayback
        };

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var totalPayback = GetDependencyValue(ScenarioKPINames.TotalPayback).Value;
            var differenceValueWithPayback = GetDependencyValue(ScenarioKPINames.DifferenceValueWithPayback).Value;

            var campaignsRevenue = _calculationContext.ActiveCampaigns.Value.Sum(c => c.RevenueBudget);

            var campaignsRevenueWithPayback = campaignsRevenue + totalPayback;

            var kpiValue = campaignsRevenueWithPayback == 0
                ? 0
                : Math.Round(differenceValueWithPayback / campaignsRevenueWithPayback * 100, 2);

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.DifferenceValuePercentagePayback,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
