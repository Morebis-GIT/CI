using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class RemainingAudienceKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public RemainingAudienceKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var kpiValue = Math.Round(_calculationContext.BaseRatings
                .Sum(be => be.BaseDemoAvailableRatings));

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.RemainingAudience,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
