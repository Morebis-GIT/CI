using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class TotalValueDeliveredKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public TotalValueDeliveredKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var bookedNominalPrice = _calculationContext.Recommendations
                .Where(x => x.Action == KPICalculationHelpers.SpotTags.Booked)
                .Sum(x => x.NominalPrice);

            var cancelledNominalPrice = _calculationContext.Recommendations
                .Where(x => x.Action == KPICalculationHelpers.SpotTags.Cancelled)
                .Sum(x => x.NominalPrice);

            var kpiValue = Math.Round(bookedNominalPrice - cancelledNominalPrice, 2);

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.TotalValueDelivered,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
