using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class RemainingAvailabilityKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public RemainingAvailabilityKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var kpiValue = _calculationContext.BaseRatings
                .Sum(be => be.TotalOpenAvailability);

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.RemainingAvailability,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
