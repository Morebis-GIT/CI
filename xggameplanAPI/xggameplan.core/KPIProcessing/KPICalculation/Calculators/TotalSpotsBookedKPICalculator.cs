using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class TotalSpotsBookedKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public TotalSpotsBookedKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var kpiValue = _calculationContext.Recommendations.Count(x =>
                x.Action == KPICalculationHelpers.SpotTags.Booked);

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.TotalSpotsBooked,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
