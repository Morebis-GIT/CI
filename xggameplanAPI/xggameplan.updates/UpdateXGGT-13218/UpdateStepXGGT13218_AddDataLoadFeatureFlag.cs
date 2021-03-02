using System;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT_13218
{
    internal class UpdateStepXGGT13218_AddDataLoadFeatureFlag : PatchUpdateStepBase, IUpdateStep
    {
        private readonly string _masterConnectionString;

        public UpdateStepXGGT13218_AddDataLoadFeatureFlag(string masterConnectionString, string updatesFolder)
        {
            _masterConnectionString = masterConnectionString;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("9AAE54EE-76E6-4AD3-9569-13EB1B466269");

        public void Apply()
        {
            using (var documentStore = DocumentStoreFactory.CreateStore(_masterConnectionString, null))
            using (var session = documentStore.OpenSession())
            {
                foreach (int tenantId in session.GetAll<Tenant>().Select(t => t.Id))
                {
                    session.Store(new TenantProductFeature
                    {
                        Name = nameof(ProductFeature.PrePostCampaignResults),
                        TenantId = tenantId,
                        IsShared = true,
                        Enabled = true
                    });
                }

                session.SaveChanges();
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGT-13218: ScenarioCampaignResults(Failures) Do Not Load to SQL Feature";

        public bool SupportsRollback => false;
    }
}
