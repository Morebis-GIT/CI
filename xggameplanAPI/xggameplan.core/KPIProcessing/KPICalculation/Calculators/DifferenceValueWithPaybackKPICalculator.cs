using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class DifferenceValueWithPaybackKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public DifferenceValueWithPaybackKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override HashSet<string> DependenciesDefinition { get; } = new HashSet<string>
        {
            ScenarioKPINames.TotalNominalValue,
            ScenarioKPINames.TotalPayback
        };

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var totalPayback = GetDependencyValue(ScenarioKPINames.TotalPayback).Value;
            var totalNominalValue = GetDependencyValue(ScenarioKPINames.TotalNominalValue).Value;

            var campaignsRevenue = _calculationContext.ActiveCampaigns.Value.Sum(c => c.RevenueBudget);

            var campaignsRevenueWithPayback = campaignsRevenue + totalPayback;

            var kpiValue = Math.Round(totalNominalValue - campaignsRevenueWithPayback, 2);

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.DifferenceValueWithPayback,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
