using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT12198_AddNewKPIsToBRSTemplates : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT12198_AddNewKPIsToBRSTemplates(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("27A50D8F-8635-4C35-AC5D-929FEBB3CC42");

        private const int ExcludedPriorityId = 1;
        private const string AverageSpotsDeliveredPerDayKpiName = "averageSpotsDeliveredPerDay";
        private const string TotalZeroRatedSpotsKpiName = "totalZeroRatedSpots";
        private const string TotalValueDeliveredKpiName = "totalValueDelivered";

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
                        template.LastModified = DateTime.UtcNow;

                        if (!template.KPIConfigurations.Exists(x => x.KPIName == AverageSpotsDeliveredPerDayKpiName))
                        {
                            template.KPIConfigurations.Add(new BRSConfigurationForKPI
                            {
                                KPIName = AverageSpotsDeliveredPerDayKpiName,
                                PriorityId = ExcludedPriorityId
                            });
                        }

                        if (!template.KPIConfigurations.Exists(x => x.KPIName == TotalZeroRatedSpotsKpiName))
                        {
                            template.KPIConfigurations.Add(new BRSConfigurationForKPI
                            {
                                KPIName = TotalZeroRatedSpotsKpiName,
                                PriorityId = ExcludedPriorityId
                            });
                        }

                        if (!template.KPIConfigurations.Exists(x => x.KPIName == TotalValueDeliveredKpiName))
                        {
                            template.KPIConfigurations.Add(new BRSConfigurationForKPI
                            {
                                KPIName = TotalValueDeliveredKpiName,
                                PriorityId = ExcludedPriorityId
                            });
                        }
                    });

                    session.SaveChanges();
                }
            }
        }

        public int Sequence => 2;

        public string Name => "XGGT-12198: Add newly added KPIs to existing BRS templates";

        public bool SupportsRollback => false;

        public void RollBack() => throw new NotImplementedException();

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }
    }
}
