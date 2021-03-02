using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class AverageEfficiencyKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public AverageEfficiencyKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var kpiValue = _calculationContext.Recommendations
                .Where(x => x.Action == KPICalculationHelpers.SpotTags.Booked)
                .Average(x => x.SpotEfficiency);

            kpiValue = Math.Round(kpiValue, 2);

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.AverageEfficiency,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
