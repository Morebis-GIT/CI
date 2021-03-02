using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT13154_AddTimestampOfLastUserModificationToScenario : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT13154_AddTimestampOfLastUserModificationToScenario(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("1a50cf20-0eaa-4326-8bc8-8417b92116af");
        public string Name => "XGGT-13154: Add timestamp of last user's modification to scenario";
        public int Sequence => 1;

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var scenarios = session.GetAll<Scenario>();

                    scenarios.ForEach(x => x.DateUserModified = x.DateModified ?? DateTime.UtcNow);

                    session.SaveChanges();
                }
            }
        }

        public bool SupportsRollback => false;
        public void RollBack() => throw new NotImplementedException();
    }
}
