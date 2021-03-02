using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Infrastructure;

namespace xggameplan.KPIProcessing.KPICalculation.Calculators
{
    public class ConversionEfficiencyByDemosKPICalculator : KPICalculatorBase
    {
        private readonly IKPICalculationContext _calculationContext;

        public ConversionEfficiencyByDemosKPICalculator(IKPICalculationContext calculationContext)
        {
            _calculationContext = calculationContext;
        }

        protected override IReadOnlyCollection<KPI> Calculate()
        {
            var kpis = new List<KPI>();

            const int zero = 0;

            foreach (var demoShortName in _calculationContext.ConversionEfficiencyDemos)
            {
                var demographic = _calculationContext.Snapshot.AllDemographics.Value
                    .FirstOrDefault(demo => demo.ShortName == demoShortName);

                if (demographic is null)
                {
                    continue;
                }

                var kpiValue = GetConversionEfficiencyByDemographicId(demographic.Id);

                kpis.Add(new KPI
                {
                    Name = ScenarioKPINames.ConversionEfficiencyByDemo + demographic.ShortName,
                    Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                    Value = kpiValue
                });
            }

            kpis.Add(new KPI
            {
                Name = ScenarioKPINames.ConversionEfficiencyTotal,
                Displayformat = KPICalculationHelpers.DisplayFormats.LargeNumber,
                Value = GetConversionEfficiencyByDemographicId(zero)
            });

            return kpis;
        }

        private double GetConversionEfficiencyByDemographicId(int demographicId) =>
            Math.Round(_calculationContext.ConversionEfficiencies
                    .Where(conversionEfficiency => conversionEfficiency.DemographicNumber == demographicId)
                    .Sum(conversionEfficiency => conversionEfficiency.ConversionEfficiencyIndex), 2, MidpointRounding.AwayFromZero);
    }
}
