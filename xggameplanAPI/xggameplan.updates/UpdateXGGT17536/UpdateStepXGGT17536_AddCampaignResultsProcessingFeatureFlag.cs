using System;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT17536
{
    internal class UpdateStepXGGT17536_AddCampaignResultsProcessingFeatureFlag : PatchUpdateStepBase, IUpdateStep
    {
        private readonly string _masterConnectionString;

        public UpdateStepXGGT17536_AddCampaignResultsProcessingFeatureFlag(string masterConnectionString, string updatesFolder)
        {
            _masterConnectionString = masterConnectionString;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("9cd16532-6475-4b7e-a9f2-5011fcedeca4");

        public void Apply()
        {
            using (var documentStore = DocumentStoreFactory.CreateStore(_masterConnectionString, null))
            using (var session = documentStore.OpenSession())
            {
                var existingFlag = session
                    .GetAll<TenantProductFeature>()
                    .FirstOrDefault(x => x.Name == nameof(ProductFeature.ScenarioCampaignResultsProcessing));

                if (existingFlag != null)
                {
                    return;
                }

                foreach (int tenantId in session.GetAll<Tenant>().Select(t => t.Id))
                {
                    session.Store(new TenantProductFeature
                    {
                        Name = nameof(ProductFeature.ScenarioCampaignResultsProcessing),
                        TenantId = tenantId,
                        IsShared = false,
                        Enabled = true
                    });
                }

                session.SaveChanges();
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGT-17536";

        public bool SupportsRollback => false;
    }
}
