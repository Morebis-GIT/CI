using System;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates.UpdateXGGT11538
{
    internal class UpdateStepXGGT11538_FeatureFlag_StrikeWeightDayPartsMerge : PatchUpdateStepBase, IUpdateStep
    {
        private readonly string _masterConnectionString;

        public UpdateStepXGGT11538_FeatureFlag_StrikeWeightDayPartsMerge(string masterConnectionString, string updatesFolder)
        {
            _masterConnectionString = masterConnectionString;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("4EE02D1A-7375-473E-8759-D4397E19DD70");

        public int Sequence => 1;

        public string Name => "XGGT-11538";

        public bool SupportsRollback => false;

        public void Apply()
        {
            using (var documentStore = DocumentStoreFactory.CreateStore(_masterConnectionString, null))
            using (var session = documentStore.OpenSession())
            {
                foreach (int tenantId in session.GetAll<Tenant>().Select(t => t.Id))
                {
                    session.Store(new TenantProductFeature
                    {
                        Name = nameof(ProductFeature.StrikeWeightDayPartsMerge),
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
