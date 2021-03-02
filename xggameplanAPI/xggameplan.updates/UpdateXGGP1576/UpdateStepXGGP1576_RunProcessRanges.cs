using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGP1576_RunProcessRanges : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGP1576_RunProcessRanges(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("1b4eac90-732a-46a7-9766-4ee05a146c84");

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    var runs = session.GetAll<Run>();
                    foreach (var run in runs)
                    {
                        if (run.ISR)
                        {
                            run.ISRDateRange = new DateRange()
                                {Start = run.StartDate.Date, End = run.EndDate.Date};
                        }

                        if (run.Smooth)
                        {
                            run.SmoothDateRange = new DateRange()
                                { Start = run.StartDate.Date, End = run.EndDate.Date };
                        }

                        if (run.Optimisation)
                        {
                            run.OptimisationDateRange = new DateRange()
                                { Start = run.StartDate.Date, End = run.EndDate.Date };
                        }

                        if (run.RightSizer)
                        {
                            run.RightSizerDateRange = new DateRange()
                                { Start = run.StartDate.Date, End = run.EndDate.Date };
                        }
                    }

                    session.SaveChanges();
                }
            }
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        public int Sequence => 1;

        public string Name => "XGGP-1576 : Run Process Ranges";

        public bool SupportsRollback => false;

        private void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, throwOnInvalid: true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, throwOnInvalid: true);
        }
    }
}
