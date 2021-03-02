using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT18062
{
    internal class UpdateStepXGGT18062_TypoInFailureDescription : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateStepXGGT18062_TypoInFailureDescription(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            var rollBackFolder = Path.Combine(updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(rollBackFolder);
        }

        public Guid Id => new Guid("9e1f5d85-1e8e-4669-9063-982f1cf3473d");

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                {
                    using (IDocumentSession session = documentStore.OpenSession())
                    {
                        var functionalArea = session
                            .GetAll<FunctionalArea>()
                            .Where(a => a.Description.Values.Contains("Slotting Controls"))
                            .FirstOrDefault();

                        if (functionalArea != null)
                        {
                            var faultType = functionalArea.FaultTypes
                                            .Where(b => b.Description.Values.Contains("Miniumum Break Availability"))
                                            .Select(c =>
                                            {
                                                c.Description["ARA"] = "Minimum Break Availability";
                                                c.Description["ENG"] = "Minimum Break Availability";
                                                return c;
                                            })
                                            .FirstOrDefault();

                            session.SaveChanges();
                        }
                    }
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();

        public int Sequence => 1;

        public string Name => "XGGT-18062";

        public bool SupportsRollback => false;
    }
}
