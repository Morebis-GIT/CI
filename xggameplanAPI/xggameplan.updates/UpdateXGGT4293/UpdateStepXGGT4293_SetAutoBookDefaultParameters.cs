using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories;
using Raven.Client;

namespace xggameplan.Updates
{
    internal partial class UpdateStepXGGT4293_SetAutoBookDefaultParameters : PatchUpdateStepBase, IUpdateStep
    {
        private readonly string _rollBackFolder;
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;

        public UpdateStepXGGT4293_SetAutoBookDefaultParameters(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("8d1726e5-ff7d-4b7d-979d-81b98c445b4d");

        public string Name => "XGGT-4293 : Set AutoBook Default Parameters";

        public int Sequence => 1;

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    var autoBookDefaultParametersRepository = new RavenAutoBookDefaultParametersRepository(session);

                    autoBookDefaultParametersRepository.AddOrUpdate(GetAutoBookDefaultParameters());

                    autoBookDefaultParametersRepository.SaveChanges();
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }
    }
}
