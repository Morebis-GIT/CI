using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Services;
using Raven.Client;
using xggameplan.Model;

namespace xggameplan.Updates
{
    /// <summary>
    /// Update step XGGP-885. Fixes scenarios/passes where the pass is referenced by multiple scenarios. This was a bug in XG28 due to holes in
    /// pre-save validation. E.g. Mulesoft could upload the Run document (containing the default scenario) but the PassIDs would not be cleared
    /// and the scenarios could be saved.
    /// </summary>
    internal class UpdateXGGP885_XGGP885 : CodeUpdateStepBase, IUpdateStep
    {

        private List<string> _tenantConnectionStrings;
        private string _updatesFolder;
        private string _rollBackFolder;

        public UpdateXGGP885_XGGP885(List<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id
        {
            get { return new Guid("2c02bd14-fd7f-4d8a-a755-5d4311ac6c92"); }
        }

        public int Sequence
        {
            get { return 1; }
        }

        public string Name
        {
            get { return "XGGP-885"; }
        }

        /// <summary>
        /// Fixes scenarios/passes where the pass is referenced by multiple scenarios.
        /// </summary>
        public void Apply()
        {            
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (IDocumentSession session = documentStore.OpenSession())
                using(var identityGenerator = new RavenIdentityGenerator(documentStore))
                {
                    // Get scenario/pass repositories
                    var scenarioRepository = new RavenScenarioRepository(session);
                    var passRepository = new RavenPassRepository(session);

                    // Get all scenarios
                    var scenarios = scenarioRepository.GetAll().OrderBy(s => s.CustomId);

                    // Check each scenario and passes
                    Dictionary<int, Guid> firstScenarioIdByPassId = new Dictionary<int, Guid>();
                    foreach (var scenario in scenarios)
                    {
                        bool scenarioChanged = false;

                        // Check each pass
                        foreach (var passRef in scenario.Passes)
                        {
                            if (firstScenarioIdByPassId.ContainsKey(passRef.Id))        // Another scenario references this pass
                            {
                                // Load pass
                                var pass = passRepository.Get(passRef.Id);

                                // Clone pass with new ID
                                var newPass = (Pass)pass.Clone();
                                newPass.Id = identityGenerator.GetIdentities<PassIdIdentity>(1)[0].Id;
                                newPass.Name = string.Format("Pass {0}", newPass.Id);
                                passRef.Id = newPass.Id;    // Link scenario to this pass
                                
                                passRepository.Add(newPass);
                                scenarioRepository.Update(scenario);
                                scenarioChanged = true;
                            }
                            else    // No other scenarios reference this pass
                            {
                                firstScenarioIdByPassId.Add(passRef.Id, scenario.Id);
                            }
                        }

                        // Save changes
                        if (scenarioChanged)
                        {
                            passRepository.SaveChanges();
                            scenarioRepository.SaveChanges();
                        }
                    }
                }
            }
        }

        public bool SupportsRollback
        {
            get { return false; }
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }
    }
}
