using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;

namespace xggameplan.Updates
{
    internal class UpdateXGGT12501_AddOutputFile : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateXGGT12501_AddOutputFile(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("1b261736-68ac-4391-bb6a-690f9d7d7999");

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (var session = documentStore.OpenSession())
                {
                    var ScenariocampaignResultsOutputFile = new OutputFile
                    {
                        FileId = "XG_CAMP_FAIL_REPT.out",
                        Description = "Scenario Campaign Failures",
                        AutoBookFileName = "XG_CAMP_FAIL_REPT.out"
                    };
                    session.Store(ScenariocampaignResultsOutputFile);
                    session.SaveChanges();
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGT-12501: Add Output File";

        public bool SupportsRollback => false;
    }
}
