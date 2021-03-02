using System;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using xggameplan.Updates;

namespace xggameplan.updates
{
    internal class UpdateXGGT12543_AddRunTypeFeatureFlag : PatchUpdateStepBase, IUpdateStep
    {
        private readonly string _masterConnectionString;
        public UpdateXGGT12543_AddRunTypeFeatureFlag(string masterConnectionString, string updatesFolder)
        {
            _masterConnectionString = masterConnectionString;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("8d2b8c95-57ea-425f-8c4e-8d8c5bb05702");

        public int Sequence => 1;

        public string Name => "XGGT-12543: Add run type feature flag";

        public bool SupportsRollback => throw new NotImplementedException();

        public void Apply()
        {
            using (var documentStore = DocumentStoreFactory.CreateStore(_masterConnectionString, null))
            using (var session = documentStore.OpenSession())
            {
                foreach (int tenantId in session.GetAll<Tenant>().Select(t => t.Id))
                {
                    session.Store(new TenantProductFeature
                    {
                        Name = nameof(ProductFeature.RunType),
                        TenantId = tenantId,
                        IsShared = true,
                        Enabled = false
                    });
                }

                session.SaveChanges();
            }
        }

        public void RollBack() => throw new NotImplementedException();
    }
}
