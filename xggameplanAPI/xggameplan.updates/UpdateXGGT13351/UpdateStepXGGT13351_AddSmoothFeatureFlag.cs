using System;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT13351
{
    internal class UpdateStepXGGT13351_AddSmoothFeatureFlag : PatchUpdateStepBase, IUpdateStep
    {
        private readonly string _masterConnectionString;
        public UpdateStepXGGT13351_AddSmoothFeatureFlag(string masterConnectionString, string updatesFolder)
        {
            _masterConnectionString = masterConnectionString;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("bd8d3059-6313-4a4b-9cf0-3f93b2fdd040");

        public int Sequence => 1;

        public string Name => "XGGT-13351: Add smooth feature flag";

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
                        Name = nameof(ProductFeature.Smooth),
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
