using System;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT17476_AddSystemLogicalDateFeatureFlag : PatchUpdateStepBase, IUpdateStep
    {
        private readonly string _masterConnectionString;
        public UpdateStepXGGT17476_AddSystemLogicalDateFeatureFlag(string masterConnectionString, string updatesFolder)
        {
            _masterConnectionString = masterConnectionString;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("68f4149d-5fae-4c3e-b310-922e0d00c683");

        public int Sequence => 1;

        public string Name => "XGGT-17476: Add System Logical Date feature flag";

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
                        Name = nameof(ProductFeature.UseSystemLogicalDate),
                        TenantId = tenantId,
                        IsShared = true,
                        Enabled = true
                    });
                }

                session.SaveChanges();
            }
        }

        public void RollBack() => throw new NotImplementedException();
    }
}
