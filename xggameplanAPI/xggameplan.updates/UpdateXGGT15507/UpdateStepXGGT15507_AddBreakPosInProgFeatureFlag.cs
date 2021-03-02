using System;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates.UpdateXGGT15507
{
    internal class UpdateStepXGGT15507_AddBreakPosInProgFeatureFlag : PatchUpdateStepBase, IUpdateStep
    {
        private readonly string _masterConnectionString;
        public UpdateStepXGGT15507_AddBreakPosInProgFeatureFlag(string masterConnectionString, string updatesFolder)
        {
            _masterConnectionString = masterConnectionString;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("C414A6D8-FFAE-498A-817C-450B6F76D480");

        public int Sequence => 1;

        public string Name => "XGGT-15507";

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
                        Name = nameof(ProductFeature.UseBreakPositionInProgram),
                        TenantId = tenantId,
                        IsShared = false,
                        Enabled = false
                    });
                }

                session.SaveChanges();
            }
        }

        public void RollBack() => throw new NotImplementedException();
    }
}
