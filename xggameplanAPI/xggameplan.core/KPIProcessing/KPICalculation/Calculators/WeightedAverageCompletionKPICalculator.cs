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
    public class WeightedAverageCompletionKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public WeightedAverageCompletionKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var requirementsSum = _calculationContext.CampaignLevels.Sum(c => c.Requirement);
            var kpiValue = _calculationContext.CampaignLevels
                .Sum(c => GetWeightedAverageCompletion(c, requirementsSum));

            kpiValue = Math.Round(kpiValue, 2);

            return new[] {
                new KPI
                {
                    Name = ScenarioKPINames.WeightedAverageCompletion,
                    Displayformat = KPICalculationHelpers.DisplayFormats.Percentage,
                    Value = kpiValue
                }};
        }

        private double GetWeight(CampaignsReqm c, double requirementsSum) =>
            c.Requirement / requirementsSum;

        private double GetAverageCompletion(CampaignsReqm c) =>
            c.Requirement == default
                ? 0
                : c.TotalSupplied / c.Requirement * 100;

        private double GetWeightedAverageCompletion(CampaignsReqm c, double requirementsSum) =>
            GetAverageCompletion(c) * GetWeight(c, requirementsSum);
    }
}
