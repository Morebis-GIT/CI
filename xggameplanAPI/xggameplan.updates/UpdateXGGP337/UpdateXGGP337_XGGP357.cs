using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using Raven.Abstractions.Data;
using Raven.Client;

namespace xggameplan.Updates
{
    /// <summary>
    /// Update step XGGP-357. Converts old Run documents to new format where scenarios & passes are stored as separate documents
    /// </summary>
    internal class UpdateStepXGGP337_XGGP357 : CodeUpdateStepBase, IUpdateStep
    {
        private List<string> _tenantConnectionStrings;
        private string _updatesFolder;
        private string _rollBackFolder;

        public UpdateStepXGGP337_XGGP357(List<string> tenantConnectionStrings, string updatesFolder)
        {
            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id
        {
            get { return new Guid("e17ec452-6250-475d-9d17-04940478f932"); }
        }

        public int Sequence
        {
            get { return 1; }
        }

        public string Name
        {
            get { return "XGGP-357"; }
        }

        /// <summary>
        /// Converts run from V1 format to new format
        /// </summary>
        /// <param name="runOld"></param>
        /// <param name="runs"></param>
        /// <param name="scenarios"></param>
        /// <param name="passes"></param>
        private void ConvertRun(RunV1 runOld, List<Run> runs, List<Scenario> scenarios, List<Pass> passes)
        {
            Run run = new Run()
            {
                Author = runOld.Author,
                Campaigns = runOld.Campaigns,
                CreatedDateTime = runOld.CreatedDateTime,
                CustomId = runOld.CustomId,
                Description = runOld.Description,
                EndDate = runOld.EndDateTime,
                ExecuteStartedDateTime = runOld.ExecuteStartedDateTime,
                Id = runOld.Id,
                IsLocked = runOld.IsLocked,
                ISR = runOld.ISR,
                LastModifiedDateTime = runOld.LastModifiedDateTime,
                Optimisation = runOld.Optimisation,
                Real = runOld.Real,
                RightSizer = runOld.RightSizer,
                //SalesAreas = runOld.SalesAreas,   //commented out for compiling as new run model for xggp-752 does not contain SalesAreas (replaced with SalesAreaPriorities)
                Smooth = runOld.Smooth,
                StartDate = runOld.StartDateTime
            };

            // Convert scenarios
            foreach (var scenarioOld in runOld.Scenarios)
            {
                RunScenario runScenario = new RunScenario()
                {
                    Id = scenarioOld.Id,
                    CompletedDateTime = scenarioOld.CompletedDateTime,
                    Progress = scenarioOld.Progress,
                    StartedDateTime = scenarioOld.StartedDateTime,
                    Status = scenarioOld.Status
                };

                Scenario scenario = new Scenario()
                {
                    CustomId = scenarioOld.CustomId,
                    Id = scenarioOld.Id,
                    Name = string.Format("Scenario {0}", scenarioOld.CustomId),  // Default
                    Passes = new List<PassReference>()
                };
                run.Scenarios.Add(runScenario);

                // Convert passes
                foreach (var passOld in scenarioOld.Passes)
                {
                    PassReference passReference = new PassReference()
                    {
                        Id = passOld.Id
                    };
                    scenario.Passes.Add(passReference);

                    Pass pass = (Pass)passOld.Clone();
                    pass.Name = string.Format("Pass {0}", pass.Id);  // Default
                    passes.Add(pass);
                }

                scenarios.Add(scenario);
            }

            runs.Add(run);
        }

        /// <summary>
        /// Converts Run documents from old format to new format where scenarios & passes are stored in separate document
        /// </summary>
        public void Apply()
        {
            //string targetTenantConnectionStringForTesting = "Url = http://18.130.103.86:8080;Database=ImagineDevChris";
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                {
                    using (IDocumentSession session = documentStore.OpenSession())
                    {
                        // Get all Run documents
                        var runDocuments = session.Advanced.DocumentStore.DatabaseCommands.Query("Raven/DocumentsByEntityName",
                                                                 new IndexQuery
                                                                 {
                                                                     Query = "Tag:[[Runs]]",
                                                                     PageSize = 1024
                                                                 });

                        List<Run> runs = new List<Run>();
                        List<Scenario> scenarios = new List<Scenario>();
                        List<Pass> passes = new List<Pass>();

                        foreach (var runDocument in runDocuments.Results)
                        {
                            string runJson = runDocument.ToString();
                            RunV1 runOld = Newtonsoft.Json.JsonConvert.DeserializeObject<RunV1>(runJson);

                            // Set RunID
                            var metadata = runDocument["@metadata"];
                            var idToken = metadata.SelectToken("@id");
                            runOld.Id = new Guid(idToken.ToObject<string>().Split('/')[1]);

                            // Back up Run document
                            if (!String.IsNullOrEmpty(_rollBackFolder))
                            {
                                string runFile = Path.Combine(_rollBackFolder, string.Format("Run.{0}.json", runOld.Id));
                                if (File.Exists(runFile))
                                {
                                    File.Delete(runFile);
                                }
                                File.WriteAllText(runFile, runJson);
                            }

                            // Convert run
                            ConvertRun(runOld, runs, scenarios, passes);
                        }

                        // Delete old runs
                        _ = session.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex("Raven/DocumentsByEntityName", new IndexQuery
                        {
                            Query = "Tag:[[Runs]]"
                        }).WaitForCompletion();

                        using (IDocumentStore targetDocumentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                        {
                            // Save new data
                            using (IDocumentSession session1 = targetDocumentStore.OpenSession())
                            {
                                passes.ForEach(pass => session1.Store(pass));
                                session1.SaveChanges();
                            }
                            using (IDocumentSession session2 = targetDocumentStore.OpenSession())
                            {
                                scenarios.ForEach(scenario => session2.Store(scenario));
                                session2.SaveChanges();
                            }
                            using (IDocumentSession session3 = targetDocumentStore.OpenSession())
                            {
                                runs.ForEach(run => session3.Store(run));
                                session3.SaveChanges();
                            }
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
