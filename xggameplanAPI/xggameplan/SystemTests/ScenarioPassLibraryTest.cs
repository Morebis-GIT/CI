using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Extensions;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;

namespace xggameplan.SystemTests
{
    /// <summary>
    /// Tests Scenario/Pass library
    /// </summary>
    internal class ScenarioPassLibraryTest : ISystemTest
    {
        private IRepositoryFactory _repositoryFactory;
        private const string _category = "Scenario/Pass Library";

        public ScenarioPassLibraryTest(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public bool CanExecute(SystemTestCategories systemTestCategories)
        {
            return true;
        }

        public List<SystemTestResult> Execute(SystemTestCategories systemTestCategory)
        {
            var results = new List<SystemTestResult>();

            try
            {
                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    var repositories = scope.CreateRepositories(
                        typeof(IPassRepository),
                        typeof(IRunRepository),
                        typeof(IScenarioRepository),
                        typeof(ITenantSettingsRepository)
                    );
                    var passRepository = repositories.Get<IPassRepository>();
                    var runRepository = repositories.Get<IRunRepository>();
                    var scenarioRepository = repositories.Get<IScenarioRepository>();
                    var tenantSettingsRepository = repositories.Get<ITenantSettingsRepository>();

                    var librariedScenarios = scenarioRepository.GetLibraried();
                    var defaultScenarioId = tenantSettingsRepository.Get().DefaultScenarioId;

                    // Check default scenario
                    if (defaultScenarioId == Guid.Empty)    // Not set
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "Default scenario is not set", ""));
                    }
                    else    // Set, check that it exists
                    {
                        var defaultScenario = scenarioRepository.Get(defaultScenarioId);
                        if (defaultScenario == null)
                        {
                            results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "Default scenario does not exist", ""));
                        }
                    }

                    // Load runs & scenarios
                    var runs = runRepository.GetAll().OrderBy(r => r.CreatedDateTime).ThenBy(r => r.Description);
                    var scenarios = scenarioRepository.GetAll().OrderBy(s => s.Id);

                    // Check runs
                    foreach (var run in runs)
                    {
                        // Check that runs for scenario exist
                        var scenarioIdsForRun = run.Scenarios.Select(s => s.Id).ToList();
                        foreach (var scenarioId in scenarioIdsForRun)
                        {
                            var scenario = scenarios.FirstOrDefault(s => s.Id == scenarioId);
                            if (scenario == null)
                            {
                                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Run ID {0} has Scenario ID {1} that does not exist", run.Id, scenarioId), ""));
                            }
                            if (librariedScenarios.Any(x => x.Id == scenarioId))
                            {
                                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Run ID {0} has Scenario ID {1} that is libraried", run.Id, scenarioId), ""));
                            }
                            if (scenarioId == defaultScenarioId)
                            {
                                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Run ID {0} has Scenario ID {1} that is the default scenario", run.Id, scenarioId), ""));
                            }
                        }
                        if (run.Scenarios.Select(s => s.Id).Distinct().ToList().Count != run.Scenarios.Count)
                        {
                            results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Run ID {0} has duplicate scenarios", run.Id), ""));
                        }
                        if (run.Scenarios.Count == 0)
                        {
                            results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Run ID {0} has no scenarios", run.Id), ""));
                        }
                    }

                    // Check each scenario. E.g. Passes exist
                    Dictionary<int, Guid> firstScenarioByPassId = new Dictionary<int, Guid>();
                    foreach (var scenario in scenarios)
                    {
                        // Get list of Run IDs for this scenario, will be one at most
                        var runsForScenario = runs.Where(r => r.Scenarios.FindIndex(s => s.Id == scenario.Id) != -1).ToList();
                        System.Text.StringBuilder runDescriptions = new System.Text.StringBuilder("");
                        foreach (var run in runsForScenario)
                        {
                            if (runDescriptions.Length > 0)
                            {
                                runDescriptions.Append(", ");
                            }
                            runDescriptions.Append(run.Id.ToString());
                        }
                        if (runsForScenario.Count > 1)
                        {
                            results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Scenario ID {0} is used by multiple runs (Run IDs {1})", scenario.Id, runDescriptions.ToString()), ""));
                        }

                        // Check that all passes exist
                        var passesForScenario = passRepository.FindByIds(scenario.Passes.Select(p => p.Id));
                        foreach (var passReference in scenario.Passes)
                        {
                            // Check that pass isn't referenced by another scenario
                            if (firstScenarioByPassId.ContainsKey(passReference.Id))
                            {
                                if (runDescriptions.Length == 0)
                                {
                                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Scenario ID {0} has a Pass ID {1} that is also used by Scenario ID {2}", scenario.Id, passReference.Id, firstScenarioByPassId[passReference.Id]), ""));
                                }
                                else
                                {
                                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Scenario ID {0} has a Pass ID {1} that is also used by Scenario ID {2} (Run IDs: {3})", scenario.Id, passReference.Id, firstScenarioByPassId[passReference.Id], runDescriptions.ToString()), ""));
                                }
                            }
                            else
                            {
                                firstScenarioByPassId.Add(passReference.Id, scenario.Id);
                            }

                            var pass = passesForScenario.FirstOrDefault(p => p.Id == passReference.Id);
                            if (pass == null)
                            {
                                if (runDescriptions.Length == 0)
                                {
                                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Scenario ID {0} has a Pass ID {1} that does not exist", scenario.Id, passReference.Id), ""));
                                }
                                else
                                {
                                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Scenario ID {0} has a Pass ID {1} that does not exist (Run IDs: {2})", scenario.Id, passReference.Id, runDescriptions.ToString()), ""));
                                }
                            }
                        }
                        if (scenario.Passes.Select(p => p.Id).Distinct().ToList().Count != scenario.Passes.Count)
                        {
                            if (runDescriptions.Length == 0)
                            {
                                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Scenario ID {0} has duplicate passes", scenario.Id), ""));
                            }
                            else
                            {
                                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Scenario ID {0} has duplicate passes (Run IDs: {1})", scenario.Id, runDescriptions.ToString()), ""));
                            }
                        }
                        if (scenario.Passes.Count == 0)
                        {
                            if (runDescriptions.Length == 0)
                            {
                                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Scenario ID {0} has no passes", scenario.Id), ""));
                            }
                            else
                            {
                                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Scenario ID {0} has no passes (Run IDs: {1})", scenario.Id, runDescriptions.ToString()), ""));
                            }
                        }
                    }
                }
            }
            catch (System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Error checking Scenario/Pass library: {0}", exception.Message), ""));
            }
            finally
            {
                if (!results.Where(r => r.ResultType == SystemTestResult.ResultTypes.Error).Any())
                {
                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, "Scenario/Pass library OK", ""));
                }
            }
            return results;
        }
    }
}
