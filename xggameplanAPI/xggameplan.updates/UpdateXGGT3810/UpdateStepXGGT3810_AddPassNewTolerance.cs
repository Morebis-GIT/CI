using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT3810_AddPassNewTolerance : CodeUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT3810_AddPassNewTolerance(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("C51B0CB6-B878-48C1-8BDF-46FC4D4FF296");

        public int Sequence => 1;

        public string Name => "XGGT-3810";

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    var passes = session.GetAll<Pass>();
                    passes
                        .ForEach(pass =>
                        {
                            if (pass.Tolerances.All(x => x.RuleId != (int)ToleranceRuleId.Programme))
                            {
                                pass.Tolerances.Add(new Tolerance
                                {
                                    RuleId = (int)ToleranceRuleId.Programme,
                                    Description = "Programme",
                                    InternalType = "Campaign",
                                    Type = "tolerances",
                                    Value = null,
                                    Ignore = true,
                                    ForceOverUnder = ForceOverUnder.None
                                });
                            }
                        });

                    session.SaveChanges();
                }
            }
        }

        public bool SupportsRollback => false;

        public void RollBack()
        {
            throw new NotImplementedException();
        }
    }
}
