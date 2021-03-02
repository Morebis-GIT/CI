using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using ImagineCommunications.GamePlan.Persistence.RavenDb;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT12198_AddKPIComparisonConfigurationsForNewKPIs : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT12198_AddKPIComparisonConfigurationsForNewKPIs(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("5DD89B8E-4E4E-4C64-B0A6-C6C57A2ED366");

        private const string AverageSpotsDeliveredPerDayKpiName = "averageSpotsDeliveredPerDay";
        private const string TotalZeroRatedSpotsKpiName = "totalZeroRatedSpots";
        private const string TotalValueDeliveredKpiName = "totalValueDelivered";
        private const float DefaultDiscernibleDifference = 1.0f;

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var comparisonConfigs = new List<KPIComparisonConfig>
                    {
                        new KPIComparisonConfig
                        {
                            KPIName = TotalValueDeliveredKpiName,
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = true,
                            Ranked = true
                        },new KPIComparisonConfig
                        {
                            KPIName = AverageSpotsDeliveredPerDayKpiName,
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = true,
                            Ranked = true
                        },
                        new KPIComparisonConfig
                        {
                            KPIName = TotalZeroRatedSpotsKpiName,
                            DiscernibleDifference = DefaultDiscernibleDifference,
                            HigherIsBest = false,
                            Ranked = true
                        }
                    };

                    comparisonConfigs.ForEach(config => session.Store(config));
                    session.SaveChanges();
                }
            }
        }

        public int Sequence => 1;

        public string Name => "XGGT-12198: Add KPIComparisonConfigs for newly added KPIs";

        public bool SupportsRollback => false;

        public void RollBack() => throw new NotImplementedException();

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }
    }
}
