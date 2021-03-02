using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;

namespace xggameplan.core.Helpers
{
    public static class BRSHelper
    {
        private const int _baseScoringValue = 10;
        private const double _calculationCoefficient = 0.1;

        public delegate double CalculateBRSIndicatorKPIFunc(double kpiValue, double averageKPIVal, double delta, double weightingFactor);

        public static readonly List<string> KPIs = new List<string>
        {
            ScenarioKPINames.ConversionEfficiencyByDemo + "ADS",
            ScenarioKPINames.ConversionEfficiencyByDemo + "MN1634",
            ScenarioKPINames.ConversionEfficiencyByDemo + "CHD",
            ScenarioKPINames.ConversionEfficiencyByDemo + "HWCH",
            ScenarioKPINames.ConversionEfficiencyByDemo + "ADABC1",
            ScenarioKPINames.AvailableRatingsByDemo + "ADS",
            ScenarioKPINames.AvailableRatingsByDemo + "MN1634",
            ScenarioKPINames.AvailableRatingsByDemo + "CHD",
            ScenarioKPINames.AvailableRatingsByDemo + "HWCH",
            ScenarioKPINames.AvailableRatingsByDemo + "ADABC1",
            ScenarioKPINames.AverageEfficiency,
            ScenarioKPINames.AvgSpotsPerDay,
            ScenarioKPINames.DifferenceValue,
            ScenarioKPINames.DifferenceValuePercentage,
            ScenarioKPINames.DifferenceValueWithPayback,
            ScenarioKPINames.DifferenceValuePercentagePayback,
            ScenarioKPINames.Percent95To105,
            ScenarioKPINames.PercentBelow75,
            ScenarioKPINames.RemainingAudience,
            ScenarioKPINames.RemainingAvailability,
            ScenarioKPINames.StandardAverageCompletion,
            ScenarioKPINames.TotalNominalValue,
            ScenarioKPINames.TotalValueDelivered,
            ScenarioKPINames.TotalZeroRatedSpots,
            ScenarioKPINames.WeightedAverageCompletion
        };

        public static readonly Dictionary<string, CalculateBRSIndicatorKPIFunc> CalculationFunctions =
            new Dictionary<string, CalculateBRSIndicatorKPIFunc>
            {
                { ScenarioKPINames.AvailableRatingsByDemo + "ADS", ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.AvailableRatingsByDemo + "MN1634", ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.AvailableRatingsByDemo + "CHD", ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.AvailableRatingsByDemo + "HWCH", ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.AvailableRatingsByDemo + "ADABC1", ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.AverageEfficiency, ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.AvgSpotsPerDay, ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.ConversionEfficiencyByDemo + "ADS", ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.ConversionEfficiencyByDemo + "MN1634", ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.ConversionEfficiencyByDemo + "CHD", ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.ConversionEfficiencyByDemo + "HWCH", ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.ConversionEfficiencyByDemo + "ADABC1", ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.DifferenceValue, ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.DifferenceValuePercentage, ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.DifferenceValueWithPayback, ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.DifferenceValuePercentagePayback, ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.Percent95To105, ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.PercentBelow75, ScoringValueCalculationWhenLowerValueBetter },
                { ScenarioKPINames.RemainingAudience, ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.RemainingAvailability, ScoringValueCalculationWhenBiggerValueBetter},
                { ScenarioKPINames.StandardAverageCompletion, ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.TotalNominalValue, ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.TotalValueDelivered, ScoringValueCalculationWhenBiggerValueBetter },
                { ScenarioKPINames.TotalZeroRatedSpots,  ScoringValueCalculationWhenLowerValueBetter },
                { ScenarioKPINames.WeightedAverageCompletion, ScoringValueCalculationWhenBiggerValueBetter }
            };

        private static double ScoringValueCalculationWhenBiggerValueBetter(double kpiValue, double averageKPIVal, double delta, double weightingFactor)
        {
            return ((kpiValue - averageKPIVal) / (delta * _calculationCoefficient) + _baseScoringValue) * weightingFactor;
        }

        private static double ScoringValueCalculationWhenLowerValueBetter(double kpiValue, double averageKPIVal, double delta, double weightingFactor)
        {
            return ((averageKPIVal - kpiValue) / (delta * _calculationCoefficient) + _baseScoringValue) * weightingFactor;
        }
    }
}
