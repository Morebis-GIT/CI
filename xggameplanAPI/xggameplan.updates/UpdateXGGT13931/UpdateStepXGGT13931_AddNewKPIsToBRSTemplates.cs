using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT13931_AddNewKPIsToBRSTemplates : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT13931_AddNewKPIsToBRSTemplates(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("C0E001F4-47FE-4F9C-8509-1530311A0FAB");

        private const int ExcludedPriorityId = 1;
        private readonly string[] _newKpiNames = {
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
            ScenarioKPINames.DifferenceValue,
            ScenarioKPINames.DifferenceValuePercentage,
            ScenarioKPINames.DifferenceValueWithPayback,
            ScenarioKPINames.DifferenceValuePercentagePayback,
            ScenarioKPINames.RatingCampaignsRatedSpots,
            ScenarioKPINames.ReservedRatingsByDemo + "ADS",
            ScenarioKPINames.SpotCampaignsRatedSpots,
            ScenarioKPINames.TotalNominalValue,
            ScenarioKPINames.TotalValueDelivered,
            ScenarioKPINames.TotalZeroRatedSpots,
            ScenarioKPINames.TotalSpotsBooked
        };

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var templates = session.GetAll<BRSConfigurationTemplate>();

                    templates.ForEach(template =>
                    {
                        foreach (var kpiName in _newKpiNames)
                        {
                            if (template.KPIConfigurations.Exists(x => x.KPIName == kpiName))
                            {
                                continue;
                            }

                            template.KPIConfigurations.Add(new BRSConfigurationForKPI
                            {
                                KPIName = kpiName,
                                PriorityId = ExcludedPriorityId
                            });
                        }

                        template.LastModified = DateTime.UtcNow;
                    });

                    session.SaveChanges();
                }
            }
        }

        public int Sequence => 1;
        public string Name => "XGGT-13931 Add newly added KPIs to existing BRS templates";

        public bool SupportsRollback => false;

        public void RollBack() => throw new NotImplementedException();

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }
    }
}
