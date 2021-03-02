using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT16136_RemoveKPIsFromBRSTemplates : IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT16136_RemoveKPIsFromBRSTemplates(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("b7d99a4c-281c-414c-b0e8-dea97b5b5351");

        private const int ExcludedPriorityId = 1;
        private readonly string[] _kpiNames = {
            ScenarioKPINames.ReservedRatingsByDemo + "ADS",
            ScenarioKPINames.RatingCampaignsRatedSpots,
            ScenarioKPINames.SpotCampaignsRatedSpots,
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
                        _ = template.KPIConfigurations.RemoveAll(x => _kpiNames.Contains(x.KPIName));

                        template.LastModified = DateTime.UtcNow;
                    });

                    session.SaveChanges();
                }
            }
        }

        public int Sequence => 1;
        public string Name => "XGGT-16136 Remove requested KPIs from existing BRS templates";

        public bool SupportsRollback => true;

        public void RollBack()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var templates = session.GetAll<BRSConfigurationTemplate>();

                    templates.ForEach(template =>
                    {
                        foreach (var kpiName in _kpiNames)
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

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }
    }
}
