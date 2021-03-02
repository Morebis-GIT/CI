using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using xggameplan.Updates.UpdateXGGT10214_Models;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT10214_UpdatePassTarpsEmptyCollection : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT10214_UpdatePassTarpsEmptyCollection(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("ef9baf7c-6a8c-4165-8b6d-473d11ee4ec6");

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    var passesWithNullTarps = session.GetAll<PassV1>()
                         .FindAll(p => p.Tarps == null);

                    passesWithNullTarps.ForEach(p => p.Tarps = new List<TarpV1>());

                    session.SaveChanges();
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGT-10214: Update passes with null tarps. Set empty tarps collection";

        public bool SupportsRollback => false;
    }
}
