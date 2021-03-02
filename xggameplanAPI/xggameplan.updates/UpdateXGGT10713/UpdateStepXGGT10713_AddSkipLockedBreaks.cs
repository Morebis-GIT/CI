using System;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT10713
{
    internal class UpdateStepXGGT10713_AddSkipLockedBreaks : PatchUpdateStepBase, IUpdateStep
    {
        private readonly string _masterConnectionString;

        public UpdateStepXGGT10713_AddSkipLockedBreaks(string masterConnectionString, string updatesFolder)
        {
            _masterConnectionString = masterConnectionString;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("6BB4ECF7-60DE-43CA-B79A-EE693A9C6FEE");

        public int Sequence => 1;

        public string Name => "XGGT-10713";

        public bool SupportsRollback => false;

        public void Apply()
        {
            using (var documentStore = DocumentStoreFactory.CreateStore(_masterConnectionString, null))
            using (var session = documentStore.OpenSession())
            {
                // add all features with disabled state
                foreach (int tenantId in session.GetAll<Tenant>().Select(t => t.Id))
                {
                    session.Store(new TenantProductFeature
                    {
                        Name = nameof(ProductFeature.SkipLockedBreaks),
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
