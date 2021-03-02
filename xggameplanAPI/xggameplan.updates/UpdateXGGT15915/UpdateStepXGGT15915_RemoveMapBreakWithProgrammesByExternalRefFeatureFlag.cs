using System;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT15915
{
    internal class UpdateStepXGGT15915_RemoveMapBreakWithProgrammesByExternalRefFeatureFlag : PatchUpdateStepBase, IUpdateStep
    {
        private readonly string _masterConnectionString;

        public UpdateStepXGGT15915_RemoveMapBreakWithProgrammesByExternalRefFeatureFlag(string masterConnectionString, string updatesFolder)
        {
            _masterConnectionString = masterConnectionString;

            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("A9D69130-3246-45F9-896E-A88116BC60FC");

        public int Sequence => 1;

        public string Name => "XGGT-15915";

        public bool SupportsRollback => throw new NotImplementedException();

        public void Apply()
        {
            using (var documentStore = DocumentStoreFactory.CreateStore(_masterConnectionString, null))
            using (var session = documentStore.OpenSession())
            {
                var featureFlags = session.Query<TenantProductFeature>()
                    .Where(x => x.Name == "MapBreakWithProgrammesByExternalRef").ToArray();

                if (featureFlags.Length != 0)
                {
                    foreach (var item in featureFlags)
                    {
                        session.Delete<TenantProductFeature>(item.Id);
                    }

                    session.SaveChanges();
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();
    }
}
