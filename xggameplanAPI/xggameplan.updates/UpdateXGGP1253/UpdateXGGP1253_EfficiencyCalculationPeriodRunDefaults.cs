using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace xggameplan.Updates
{
    internal class UpdateXGGP1253_EfficiencyCalculationPeriodRunDefaults : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateXGGP1253_EfficiencyCalculationPeriodRunDefaults(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("79d59a4a-8082-4e7f-8e12-e2094ec60e29");

        public void Apply()
        {
            foreach (var tenantConnectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (var session = documentStore.OpenSession())
                {
                    var all = session.GetAll<Run>();

                    foreach (var run in all)
                    {
                        run.EfficiencyPeriod = EfficiencyCalculationPeriod.RunPeriod;
                        run.NumberOfWeeks = null;

                        session.Store(run);
                    }

                    session.SaveChanges();
                } 
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGP-1253: Set Run Efficiency Calculation Period Defaults";

        public bool SupportsRollback => false;
    }
}
