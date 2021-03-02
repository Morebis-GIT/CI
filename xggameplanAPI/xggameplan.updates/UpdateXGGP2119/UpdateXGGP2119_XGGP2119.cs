using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories;
using Raven.Client;

namespace xggameplan.Updates
{
    internal class UpdateXGGP2119_XGGP2119 : CodeUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateXGGP2119_XGGP2119(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("595d232a-96e9-4ffe-8c0b-8aafa69188c1");

        public int Sequence => 1;

        public string Name => "XGGP-2119";

        /// <summary>
        /// Added field IsLibraried for scenarios and passes
        /// </summary>
        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                {
                    using (IDocumentSession session = documentStore.OpenSession())
                    {
                        var passRepository = new RavenPassRepository(session);
                        var scenarioRepository = new RavenScenarioRepository(session);
                        var runRepository = new RavenRunRepository(session);

                        var passes = passRepository.GetAll().ToList();
                        var scenarios = scenarioRepository.GetAll().ToList();
                        var runs = runRepository.GetAll().ToList();

                        // Get scenarios linked to run
                        var scenarioWithRunIds = new Dictionary<Guid, Guid>();
                        runs.ForEach(r =>
                        {
                            r.Scenarios.ForEach(s => scenarioWithRunIds.Add(s.Id, r.Id));
                        });

                        var runScenarioIds = scenarioWithRunIds.Select(s => s.Key).ToList();

                        // Update field IsLibraried for scenarios not libked to run
                        scenarios
                            .Where(s => !runScenarioIds.Contains(s.Id))
                            .Distinct()
                            .ToList()
                            .ForEach(scenario =>
                            {
                                scenario.IsLibraried = true;
                                scenarioRepository.Update(scenario);
                            });

                        scenarios
                            .Where(s => s.IsLibraried == null || !s.IsLibraried.Value)
                            .ToList()
                            .ForEach(scenario =>
                            {
                                scenario.IsLibraried = false;
                                scenarioRepository.Update(scenario);
                            });

                        // Get default scenario id
                        // var defaultScenarioId = tenantSettingsRepository.Find(TenantSettings.DefaultId).DefaultScenarioId;

                        // Get passes linked to scenario
                        var scenarioWithPassIds = new Dictionary<int, Guid>();
                        scenarios.ForEach(s =>
                        {
                            s.Passes.ForEach(p =>
                            {
                                scenarioWithPassIds.Add(p.Id, s.Id);
                            });
                        });

                        //// Get pass ids not linked to scenarios
                        //var librariedPassIds = passes
                        //    .Select(pass => pass.Id)
                        //    .Except(scenarioWithPassIds
                        //        .Where(val => val.Value != defaultScenarioId)
                        //        .Select(s => s.Key))
                        //    .Distinct()
                        //    .ToList();
                        var librariedPassIds = passes
                            .Select(pass => pass.Id).Except(scenarioWithPassIds.Select(s => s.Key))
                            .Distinct()
                            .ToList();

                        // Update field IsLibararied for passes
                        passes
                            .Where(pass => librariedPassIds.Contains(pass.Id))
                            .Distinct()
                            .ToList()
                            .ForEach(pass =>
                            {
                                pass.IsLibraried = true;
                                passRepository.Update(pass);
                            });

                        passes
                            .Where(p => p.IsLibraried == null || !p.IsLibraried.Value)
                            .ToList()
                            .ForEach(pass =>
                            {
                                pass.IsLibraried = false;
                                passRepository.Update(pass);
                            });

                        scenarioRepository.SaveChanges();
                        passRepository.SaveChanges();
                    }
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
