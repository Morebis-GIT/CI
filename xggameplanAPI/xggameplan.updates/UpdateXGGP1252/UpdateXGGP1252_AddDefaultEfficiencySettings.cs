using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Persistence.RavenDb;

namespace xggameplan.Updates
{
    internal class UpdateXGGP1252_AddDefaultEfficiencySettings : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateXGGP1252_AddDefaultEfficiencySettings(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("3d102613-c057-4d6d-b8ed-79f70305abc0");

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (var session = documentStore.OpenSession())
                {
                    var one = 1;

                    var defaultEfficiencySettings = new EfficiencySettings
                    {
                        Id = Guid.NewGuid(),
                        DefaultNumberOfWeeks = one,
                        PersistEfficiency = PersistEfficiency.NightRun,
                        EfficiencyCalculationPeriod = EfficiencyCalculationPeriod.RunPeriod
                    };

                    session.Store(defaultEfficiencySettings);

                    session.SaveChanges();
                } 
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGP-1252: Add default efficiency settings";

        public bool SupportsRollback => false;
    }
}
