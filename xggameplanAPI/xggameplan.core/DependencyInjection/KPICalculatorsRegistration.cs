using Autofac;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.KPIProcessing.KPICalculation.Calculators;

namespace xggameplan.core.DependencyInjection
{
    public static class KPICalculatorsRegistration
    {
        public static void RegisterKPICalculators(this ContainerBuilder builder)
        {
            _ = builder.RegisterType<AvailableRatingsByDemosKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.AvailableRatingsByDemo);
            _ = builder.RegisterType<AverageCancelEfficiencyKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.AverageCancelEfficiency);
            _ = builder.RegisterType<AverageEfficiencyKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.AverageEfficiency);
            _ = builder.RegisterType<AverageSpotsDeliveredPerDayKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.AvgSpotsPerDay);
            _ = builder.RegisterType<BaseDemographRatingsKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.BaseDemographicRatings);
            _ = builder.RegisterType<ConversionEfficiencyByDemosKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.ConversionEfficiencyByDemo);
            _ = builder.RegisterType<DifferenceValueKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.DifferenceValue);
            _ = builder.RegisterType<DifferenceValuePercentageKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.DifferenceValuePercentage);
            _ = builder.RegisterType<DifferenceValueWithPaybackKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.DifferenceValueWithPayback);
            _ = builder.RegisterType<DifferenceValuePercentagePaybackKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.DifferenceValuePercentagePayback);
            _ = builder.RegisterType<NoOfCampaignsKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.NoOfCampaigns);
            _ = builder.RegisterType<Percent75to95KPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.Percent75To95);
            _ = builder.RegisterType<Percent95to105KPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.Percent95To105);
            _ = builder.RegisterType<PercentBelow75KPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.PercentBelow75);
            _ = builder.RegisterType<PercentGreater105KPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.PercentGreater105);
            _ = builder.RegisterType<RatingCampaignsRatedSpotsKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.RatingCampaignsRatedSpots);
            _ = builder.RegisterType<RemainingAudienceKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.RemainingAudience);
            _ = builder.RegisterType<RemainingAvailabilityKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.RemainingAvailability);
            _ = builder.RegisterType<ReservedRatingsByDemosKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.ReservedRatingsByDemo);
            _ = builder.RegisterType<SpotCampaignsRatedSpotsKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.SpotCampaignsRatedSpots);
            _ = builder.RegisterType<StandardAverageCompletionKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.StandardAverageCompletion);
            _ = builder.RegisterType<TotalNominalValueKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.TotalNominalValue);
            _ = builder.RegisterType<TotalPaybackKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.TotalPayback);
            _ = builder.RegisterType<TotalRatingCampaignSpotsKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.TotalRatingCampaignSpots);
            _ = builder.RegisterType<TotalRevenueKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.TotalRevenue);
            _ = builder.RegisterType<TotalSpotCampaignSpotsKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.TotalSpotCampaignSpots);
            _ = builder.RegisterType<TotalSpotsBookedKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.TotalSpotsBooked);
            _ = builder.RegisterType<TotalValueDeliveredKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.TotalValueDelivered);
            _ = builder.RegisterType<TotalZeroRatedSpotsKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.TotalZeroRatedSpots);
            _ = builder.RegisterType<WeightedAverageCompletionKPICalculator>().Keyed<KPICalculatorBase>(ScenarioKPINames.WeightedAverageCompletion);
        }
    }
}
