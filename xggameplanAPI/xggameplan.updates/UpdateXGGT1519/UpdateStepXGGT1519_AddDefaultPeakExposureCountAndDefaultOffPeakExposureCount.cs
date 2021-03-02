using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT1519_AddDefaultPeakExposureCountAndDefaultOffPeakExposureCount : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT1519_AddDefaultPeakExposureCountAndDefaultOffPeakExposureCount(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("6d6da908-c7c8-46c1-9d0c-1e906176aef2");

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var allClashes = session.GetAll<Clash>();

                    foreach (var clash in allClashes)
                    {
                        clash.DefaultPeakExposureCount =
                            clash.DefaultOffPeakExposureCount = clash.ExposureCount;

                        if (clash.Differences is null)
                        {
                            clash.Differences = new List<ClashDifference>();
                        }

                        session.Store(clash);
                    }

                    session.SaveChanges();
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGT-1519: Add default peak exposure count and default off-peak exposure count";

        public bool SupportsRollback => false;
    }
}
