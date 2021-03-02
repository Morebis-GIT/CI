using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class AverageSpotsDeliveredPerDayKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public AverageSpotsDeliveredPerDayKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override HashSet<string> DependenciesDefinition { get; } = new HashSet<string>
        {
            ScenarioKPINames.TotalSpotsBooked
        };

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var totalSpotsBooked = GetDependencyValue(ScenarioKPINames.TotalSpotsBooked).Value;

            var runDurationDays = (_calculationContext.Snapshot.Run.Value.EndDate
                                   - _calculationContext.Snapshot.Run.Value.StartDate).Days + 1;

            var kpiValue = (int)Math.Round(totalSpotsBooked / runDurationDays, MidpointRounding.AwayFromZero);

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.AvgSpotsPerDay,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
