using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT9163_NullCampaignPassPrioritiesPatch : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT9163_NullCampaignPassPrioritiesPatch(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("7f032241-1fa8-4dec-97d3-7749b0243064");

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (var session = documentStore.OpenSession())
                {
                    var scenarios = session.GetAll<Scenario>();

                    foreach (var scenario in scenarios)
                    {
                        if (scenario.CampaignPassPriorities is null)
                        {
                            scenario.CampaignPassPriorities = new List<CampaignPassPriority>();
                        }
                    }
                    session.SaveChanges();
                }
            }
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        public int Sequence => 1;

        public string Name => "XGGT-9163 : CampaignPassPriorities patch for null values";

        public bool SupportsRollback => false;

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }
    }


}
