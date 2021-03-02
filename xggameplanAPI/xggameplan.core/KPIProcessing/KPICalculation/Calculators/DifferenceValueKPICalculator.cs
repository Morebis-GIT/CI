using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class DifferenceValueKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public DifferenceValueKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override HashSet<string> DependenciesDefinition { get; } = new HashSet<string>
        {
            ScenarioKPINames.TotalNominalValue
        };

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var totalNominalValue = GetDependencyValue(ScenarioKPINames.TotalNominalValue).Value;
            var campaignsRevenue = _calculationContext.ActiveCampaigns.Value.Sum(c => c.RevenueBudget);

            var kpiValue = Math.Round(totalNominalValue - campaignsRevenue, 2);

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.DifferenceValue,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
