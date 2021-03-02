using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;
using xggameplan.OutputFiles.Processing;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class StandardAverageCompletionKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public StandardAverageCompletionKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var kpiValue = _calculationContext.CampaignLevels.Sum(GetAverageCompletion)
                           / _calculationContext.CampaignLevels.Count();

            kpiValue = Math.Round(kpiValue, 2);

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.StandardAverageCompletion,
                    Displayformat = KPICalculationHelpers.DisplayFormats.Percentage,
                    Value = kpiValue
                }};
        }

        private double GetAverageCompletion(CampaignsReqm c) =>
            c.Requirement == default
                ? 0
                : c.TotalSupplied / c.Requirement * 100;
    }
}
