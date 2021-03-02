using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT14618
{
    internal class UpdateStepXGGT14618_AgHfssDemos : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT14618_AgHfssDemos(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("A50DB882-EEAC-4D03-AD38-8816C01EF73B");

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                {
                    documentStore.Listeners.RegisterListener(new Version1ToVersion2Converter());
                    using (var session = documentStore.OpenSession())
                    {
                        var autoBookDefaultParameters = session.GetAll<AutoBookDefaultParameters>().ToList();

                        foreach (var x in autoBookDefaultParameters)
                        {
                            session.Store(x);
                        }

                        session.SaveChanges();
                    }
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGT-14618 : AgHfssDemos";

        public bool SupportsRollback => false;
    }
}
