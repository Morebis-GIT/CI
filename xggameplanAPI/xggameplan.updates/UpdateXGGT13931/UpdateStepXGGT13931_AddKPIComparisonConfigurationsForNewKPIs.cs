using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT13931_AddKPIComparisonConfigurationsForNewKPIs : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT13931_AddKPIComparisonConfigurationsForNewKPIs(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("32E92580-003C-4904-9923-01DA0E128DDC");

        private const float DefaultDiscernibleDifference = 1.0f;

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var newComparisonConfigs = new List<KPIComparisonConfig>
                    {
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.ConversionEfficiencyByDemo + "ADS",
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = false,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.ConversionEfficiencyByDemo + "MN1634",
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = false,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.ConversionEfficiencyByDemo + "CHD",
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = false,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.ConversionEfficiencyByDemo + "HWCH",
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = false,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.ConversionEfficiencyByDemo + "ADABC1",
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = false,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.AvailableRatingsByDemo + "ADS",
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = false,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.AvailableRatingsByDemo + "MN1634",
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = false,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.AvailableRatingsByDemo + "CHD",
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = false,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.AvailableRatingsByDemo + "HWCH",
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = false,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.AvailableRatingsByDemo + "ADABC1",
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = false,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.ReservedRatingsByDemo + "ADS",
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = false,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.RatingCampaignsRatedSpots,
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = true,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.SpotCampaignsRatedSpots,
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = true,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.BaseDemographicRatings,
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = true,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.TotalRatingCampaignSpots,
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = true,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.TotalSpotCampaignSpots,
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = true,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.TotalNominalValue,
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = true,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.DifferenceValue,
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = true,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.DifferenceValueWithPayback,
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = true,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.DifferenceValuePercentage,
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = true,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = ScenarioKPINames.DifferenceValuePercentagePayback,
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = true,
                            Ranked = true
                        }
                    };

                    var existingComparisonConfigs = session.GetAll<KPIComparisonConfig>();

                    _ = newComparisonConfigs.RemoveAll(x =>
                          existingComparisonConfigs.Exists(t => t.KPIName == x.KPIName));

                    newComparisonConfigs.ForEach(config => session.Store(config));
                    session.SaveChanges();
                }
            }
        }

        public int Sequence => 2;
        public string Name => "XGGT-13931: Add KPIComparisonConfigs for newly added KPIs";

        public bool SupportsRollback => false;

        public void RollBack() => throw new NotImplementedException();

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }
    }
}
