using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class AvailableRatingsByDemosKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public AvailableRatingsByDemosKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var kpis = new List<KPI>();

            foreach (var demoShortName in _calculationContext.AvailableRatingsDemos)
            {
                var demographic = _calculationContext.Snapshot.AllDemographics.Value
                    .FirstOrDefault(demo => demo.ShortName == demoShortName);

                if (demographic is null)
                {
                    continue;
                }

                var kpiValue = Math.Round(_calculationContext.ReserveRatings
                    .Where(rating => rating.DemographicNumber == demographic.Id)
                    .Sum(rating => rating.AvailableRatings), 2, MidpointRounding.AwayFromZero);

                kpis.Add(new KPI
                {
                    Name = ScenarioKPINames.AvailableRatingsByDemo + demographic.ShortName,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                });
            }

            return kpis;
        }
    }
}
