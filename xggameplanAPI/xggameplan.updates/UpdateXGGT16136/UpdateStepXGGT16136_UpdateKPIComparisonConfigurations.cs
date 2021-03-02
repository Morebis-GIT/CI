using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT16136_UpdateKPIComparisonConfigurations : IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT16136_UpdateKPIComparisonConfigurations(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("bb32d710-c604-4f6a-a3b5-6737b2416f70");

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var comparisonConfigs = session.GetAll<KPIComparisonConfig>()
                        .Where(x =>
                            x.KPIName.Contains(ScenarioKPINames.AvailableRatingsByDemo)
                            || x.KPIName.Contains(ScenarioKPINames.ConversionEfficiencyByDemo))
                        .ToArray();

                    comparisonConfigs.ForEach(x => x.HigherIsBest = true);

                    session.SaveChanges();
                }
            }
        }

        public int Sequence => 2;
        public string Name => "XGGT-16136: Update KPIComparisonConfigs for requested KPIs";

        public bool SupportsRollback => true;

        public void RollBack()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var comparisonConfigs = session.GetAll<KPIComparisonConfig>()
                        .Where(x => x.KPIName.Contains(ScenarioKPINames.AvailableRatingsByDemo))
                        .ToArray();

                    comparisonConfigs.ForEach(x => x.HigherIsBest = false);

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
