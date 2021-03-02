using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using Newtonsoft.Json;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Json.Linq;

namespace xggameplan.Updates
{
    internal class UpdateXGGT9644_RemoveClashDifferences : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateXGGT9644_RemoveClashDifferences(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("FDC212F3-D1EC-4974-AA1A-C4EADD4006EA");

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    var clashDocuments = session.Advanced.DocumentStore.DatabaseCommands.Query(
                        "Raven/DocumentsByEntityName",
                        new IndexQuery
                        {
                            Query = "Tag:[[Clashes]]",
                            PageSize = 1024
                        });

                    RemoveDifferences(clashDocuments.Results, session);
                }
            }
        }

        private void RemoveDifferences(List<RavenJObject> currentClashes, IDocumentSession session)
        {
            foreach (var clash in currentClashes)
            {
                _ = clash.Remove("Differences");
                var parsedClash = JsonConvert.DeserializeObject<Clash>(clash.ToString());
                parsedClash.Differences = new List<ClashDifference>();
                session.Store(parsedClash);
            }
            session.SaveChanges();
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        public int Sequence => 1;

        public string Name => "XGGP-9644 : It's technical script for removing clash differences";

        public bool SupportsRollback => false;

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }
    }
}
