using System;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT13949_UpdareIntegrationSyncFeature : PatchUpdateStepBase, IUpdateStep
    {
        private readonly string _masterConnectionString;

        public UpdateStepXGGT13949_UpdareIntegrationSyncFeature(string masterConnectionString, string updatesFolder)
        {
            _masterConnectionString = masterConnectionString;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("40384a3e-332b-49c6-9126-aa25eaff0ff3");

        public void Apply()
        {
            using (var documentStore = DocumentStoreFactory.CreateStore(_masterConnectionString, null))
            using (var session = documentStore.OpenSession())
            {
                var existingFlags = session
                    .GetAll<TenantProductFeature>()
                    .Where(x => x.Name == nameof(ProductFeature.IntegrationSynchronization));

                if (!existingFlags.Any())
                {
                    return;
                }

                foreach (var existingFlag in existingFlags)
                {
                    existingFlag.IsShared = true;
                }

                session.SaveChanges();
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGT-13949";

        public bool SupportsRollback => false;
    }
}
