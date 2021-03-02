using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class TotalNominalValueKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public TotalNominalValueKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override HashSet<string> DependenciesDefinition { get; } = new HashSet<string>
        {
            ScenarioKPINames.TotalValueDelivered
        };

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var totalValueDelivered = GetDependencyValue(ScenarioKPINames.TotalValueDelivered).Value;
            var existingNominalValue = _calculationContext.ActiveCampaigns.Value
                .Sum(c => c.RevenueBooked);

            var kpiValue = totalValueDelivered + existingNominalValue.Value;

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.TotalNominalValue,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                }};
        }
    }
}
