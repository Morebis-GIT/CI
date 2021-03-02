using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;

namespace xggameplan.SystemTests
{
    /// <summary>
    /// Tests a completed run. Checks the output. This is typically used internally by other run tests.
    /// </summary>
    internal class RunOutputTest : ISystemTest
    {
        private Run _run;
        private IRepositoryFactory _repositoryFactory;
        private int _scenarioCompleteTimeoutMins;
        private string _category;

        public RunOutputTest(Run run, string category, IRepositoryFactory repositoryFactory, int scenarioCompleteTimeoutMins)
        {
            _run = run;
            _repositoryFactory = repositoryFactory;
            _category = category;
            _scenarioCompleteTimeoutMins = scenarioCompleteTimeoutMins;
        }

        public bool CanExecute(SystemTestCategories systemTestCategories)
        {
            return true;
        }

        public List<SystemTestResult> Execute(SystemTestCategories systemTestCategory)
        {
            List<SystemTestResult> results = new List<SystemTestResult>();

            try
            {
                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    // Get sales areas for run
                    var salesAreaRepository = scope.CreateRepository<ISalesAreaRepository>();
                    var salesAreas = RunManagement.RunManager.GetSalesAreas(_run, salesAreaRepository.GetAll());

                    // Check scenarios
                    foreach (var scenario in _run.Scenarios)
                    {
                        // Warn of possibly crashed scenarios, running for too long
                        if (scenario.StartedDateTime != null && scenario.IsScheduledOrRunning && scenario.Status != ScenarioStatuses.Scheduled)
                        {
                            if (scenario.StartedDateTime.Value.AddMinutes(_scenarioCompleteTimeoutMins) <= DateTime.UtcNow)   // Hard-coded limit for the moment
                            {
                                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Warning, _category, string.Format("Run {0} (RunID {1}) has a scenario (ScenarioID {2}) that was started at {3} and it is still running after {4} mins." +
                                                                        "Please check if it has crashed.",
                                                                        _run.Description, _run.Id, scenario.Id, scenario.StartedDateTime, _scenarioCompleteTimeoutMins), ""));
                            }
                        }

                        switch (scenario.Status)
                        {
                            case ScenarioStatuses.CompletedError:
                                //countFailedRuns++;
                                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Run {0} (Run ID {1}) has a scenario (Scenario ID {2}) that completed with errors. The Optimiser Report will indicate why it failed.", _run.Description, _run.Id, scenario.Id), ""));
                                break;
                            case ScenarioStatuses.Scheduled:
                                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Run {0} (Run ID {1}) has a scenario (Scenario ID {2}) that has been scheduled but not started. Please check if there is a problem with the AutoBooks.", _run.Description, _run.Id, scenario.Id), ""));
                                break;
                        }

                        // Check recommendations output data
                        results.AddRange(CheckRecommendationOutputData(_run, salesAreas, scenario));

                        // Check failures output data
                        results.AddRange(CheckFailuresOutputData(_run, salesAreas, scenario));

                        // Check scenario results output data
                        results.AddRange(CheckScenarioResultsOutputData(_run, salesAreas, scenario));
                    }

                    // Check Smooth failures output data
                    results.AddRange(CheckSmoothFailuresOutputData(_run, salesAreas));

                    // Check break efficiency output data
                    results.AddRange(CheckBreakEfficiencyOutputData(_run, salesAreas));
                }
            }
            catch (System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Error checking run output (Run ID {0}): {1}", _run.Id, exception.Message), ""));
            }
            finally
            {
                if (!results.Where(r => r.ResultType == SystemTestResult.ResultTypes.Error).Any() && _run.Scenarios.Where(s => s.Status == ScenarioStatuses.CompletedSuccess).ToList().Count == _run.Scenarios.Count)
                {
                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, "Run output OK", ""));
                }
            }
            return results;
        }

        /// <summary>
        /// Checks for failures output data.
        /// </summary>
        /// <param name="run"></param>
        /// <param name="salesAreas"></param>
        /// <param name="scenario"></param>
        /// <returns></returns>
        private List<SystemTestResult> CheckFailuresOutputData(Run run, List<SalesArea> salesAreas, RunScenario scenario)
        {
            List<SystemTestResult> results = new List<SystemTestResult>();

            try
            {
                if (scenario.Status == ScenarioStatuses.CompletedSuccess)
                {
                    using (var scope = _repositoryFactory.BeginRepositoryScope())
                    {
                        var failuresRepository = scope.CreateRepository<IFailuresRepository>();
                        var failures = failuresRepository.Get(scenario.Id);
                        if (failures == null || failures.Items == null || failures.Items.Count == 0)
                        {
                            results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Run {0} (RunID {1}) has a scenario (Scenario ID {2}) that generated no Optimiser failures.", run.Description, run.Id, scenario.Id), ""));
                        }
                    }

                    if (!results.Where(r => r.ResultType == SystemTestResult.ResultTypes.Error).Any())
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, "Failures output data OK", ""));
                    }
                }
            }
            catch (System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, string.Format("Error checking Failures output data: {0}", exception.Message), ""));
            }
            return results;
        }

        /// <summary>
        /// Checks Smooth failures output data
        /// </summary>
        /// <param name="run"></param>
        /// <param name="salesAreas"></param>
        /// <returns></returns>
        private List<SystemTestResult> CheckSmoothFailuresOutputData(Run run, List<SalesArea> salesAreas)
        {
            List<SystemTestResult> results = new List<SystemTestResult>();

            try
            {
                if (run.Smooth)    // Smooth failures only exist if Smooth was run
                {
                    using (var scope = _repositoryFactory.BeginRepositoryScope())
                    {
                        var smoothFailureRepository = scope.CreateRepository<ISmoothFailureRepository>();
                        var smoothFailures = smoothFailureRepository.GetByRunId(run.Id);
                        if (smoothFailures == null)
                        {
                            results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Run {0} (RunID {1}) generated no Smooth failures. There was a problem executing Smooth processing.", run.Description, run.Id), ""));
                        }
                    }

                    if (!results.Where(r => r.ResultType == SystemTestResult.ResultTypes.Error).Any())
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, "Smooth Failures output data OK", ""));
                    }
                }
            }
            catch (System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, string.Format("Error checking Smooth Failures output data: {0}", exception.Message), ""));
            }
            return results;
        }

        /// <summary>
        /// Checks for recommendations. If recommendations are not present then it may indicate a problem.
        /// </summary>
        /// <param name="run"></param>
        /// <param name="salesAreas"></param>
        /// <param name="scenario"></param>
        /// <returns></returns>
        private List<SystemTestResult> CheckRecommendationOutputData(Run run, List<SalesArea> salesAreas, RunScenario scenario)
        {
            List<SystemTestResult> results = new List<SystemTestResult>();

            try
            {
                if (scenario.Status == ScenarioStatuses.CompletedSuccess)
                {
                    // Check if recommendations are missing
                    List<string> processors = new List<string>()
                        {
                            (run.Smooth ? "smooth" : ""),
                            (run.Optimisation ? "autobook" : ""),
                            (run.ISR ? "isr" : ""),
                            (run.RightSizer ? "rzr" : "")
                        };
                    processors.RemoveAll(p => String.IsNullOrEmpty(p));

                    Dictionary<string, string> processorDescriptions = new Dictionary<string, string>()
                        {
                            { "smooth", "Smooth" },
                            { "autobook", "Optimiser" },
                            { "isr", "ISR" },
                            { "rzr", "Right Sizer" }
                        };

                    if (processors.Any())
                    {
                        using (var scope = _repositoryFactory.BeginRepositoryScope())
                        {
                            var recommendationRepository = scope.CreateRepository<IRecommendationRepository>();

                            // Get recommendations
                            var recommendations = recommendationRepository.GetByScenarioId(scenario.Id);

                            // For each processor then check if any recommendations
                            foreach (string processor in processors)
                            {
                                var processorRecommendations = recommendations.Where(r => r.Processor == processor);
                                if (!processorRecommendations.Any())
                                {
                                    if (processor == "smooth")   // Smooth recommendations related to run, only set for first scenario
                                    {
                                        if (scenario == run.Scenarios.First())
                                        {
                                            results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Run {0} (Run ID {1}) generated no {2} recommendations for scenario {3}.", run.Description, run.Id, processorDescriptions[processor], scenario.Id), ""));
                                        }
                                    }
                                    else
                                    {
                                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Run {0} (Run ID {1}) generated no {2} recommendations for scenario {3}.", run.Description, run.Id, processorDescriptions[processor], scenario.Id), ""));
                                    }
                                }
                            }
                        }
                    }

                    if (!results.Where(r => r.ResultType == SystemTestResult.ResultTypes.Error).Any())
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, "Recommendations output data OK", ""));
                    }
                }
            }
            catch (System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, string.Format("Error checking Recommendations output data: {0}", exception.Message), ""));
            }
            return results;
        }

        /// <summary>
        /// Check scenario results data. E.g. KPI data.
        /// </summary>
        /// <param name="run"></param>
        /// <param name="salesAreas"></param>
        /// <param name="scenario"></param>
        /// <returns></returns>
        private List<SystemTestResult> CheckScenarioResultsOutputData(Run run, List<SalesArea> salesAreas, RunScenario scenario)
        {
            List<SystemTestResult> results = new List<SystemTestResult>();

            try
            {
                if (scenario.Status == ScenarioStatuses.CompletedSuccess)
                {
                    using (var scope = _repositoryFactory.BeginRepositoryScope())
                    {
                        var scenarioResultRepository = scope.CreateRepository<IScenarioResultRepository>();
                        var scenarioResult = scenarioResultRepository.Find(scenario.Id);
                        if (scenarioResult == null)     // ScenarioResult missing
                        {
                            results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Run {0} (Run ID {1}) generated no Scenario Result document for scenario {2}. There will be no KPI data. There was a problem processing the output files.", run.Description, run.Id, scenario.Id), ""));
                        }
                        else if (scenarioResult.Metrics == null || !scenarioResult.Metrics.Any())   // No KPI data
                        {
                            results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Run {0} (Run ID {1}) generated no KPI for scenario {2}. There was a problem processing the output files.", run.Description, run.Id, scenario.Id), ""));
                        }
                        else
                        {
                            // Check individual metrics
                            int countZeroMetrics = 0;
                            foreach (var kpi in scenarioResult.Metrics)
                            {
                                if (kpi.Value == 0)
                                {
                                    countZeroMetrics++;
                                }
                            }
                            if (countZeroMetrics == scenarioResult.Metrics.Count)
                            {
                                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Run {0} (Run ID {1}) generated KPI for scenario {2} but all of the values were zero. There was a problem processing the output files.", run.Description, run.Id, scenario.Id), ""));
                            }
                        }
                    }

                    if (!results.Where(r => r.ResultType == SystemTestResult.ResultTypes.Error).Any())
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, "Scenario Results out data OK", ""));
                    }
                }
            }
            catch (System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, string.Format("Error checking Scenario Results output data: {0}", exception.Message), ""));
            }
            return results;
        }

        /// <summary>
        /// Checks break efficiency data generated by run
        /// </summary>
        /// <param name="run"></param>
        /// <param name="salesAreas"></param>
        /// <returns></returns>
        private List<SystemTestResult> CheckBreakEfficiencyOutputData(Run run, List<SalesArea> salesAreas)
        {
            List<SystemTestResult> results = new List<SystemTestResult>();

            try
            {
                if (run.Scenarios.Where(s => s.Status == ScenarioStatuses.CompletedSuccess).Any())
                {
                    // Check break efficiency, find one Schedule document with it set.
                    int countSalesAreasWithBreakEfficiencySet = 0;
                    SemaphoreSlim semaphore = new SemaphoreSlim(10);   // Limit threads
                    Mutex sharedResourceMutex = new Mutex();
                    var tasks = new List<Task>();
                    foreach (var salesArea in salesAreas)
                    {
                        semaphore.Wait();       // Wait for free thread
                        var task = Task.Factory.StartNew(() =>
                        {
                            using (var scope = _repositoryFactory.BeginRepositoryScope())
                            {
                                sharedResourceMutex.WaitOne();
                                var scheduleRepository = scope.CreateRepository<IScheduleRepository>();
                                sharedResourceMutex.ReleaseMutex();

                                // Try and find a Schedule that has break efficiency set
                                bool isBreakEfficiencySetForSalesArea = false;
                                DateTime currentScheduleDate = run.StartDate.Date.AddDays(-1);
                                while (currentScheduleDate < run.EndDate.Date && !isBreakEfficiencySetForSalesArea && countSalesAreasWithBreakEfficiencySet == 0)
                                {
                                    currentScheduleDate = currentScheduleDate.AddDays(1);
                                    var schedule = scheduleRepository.GetSchedule(salesArea.Name, currentScheduleDate.Date);
                                    if (schedule != null && schedule.Breaks != null)
                                    {
                                        // See if there's a break with efficiency set
                                        var breakWithEfficiency = schedule.Breaks.Find(b => b.BreakEfficiencyList != null && b.BreakEfficiencyList.Count > 0 && b.BreakEfficiencyList.Where(bei => bei.Efficiency > 0).Any());
                                        if (breakWithEfficiency != null)
                                        {
                                            Interlocked.Increment(ref countSalesAreasWithBreakEfficiencySet);
                                            isBreakEfficiencySetForSalesArea = true;
                                        }
                                    }
                                }
                            }
                        }).ContinueWith(myTask =>
                        {
                            semaphore.Release();        // Release thread
                            if (myTask.Exception != null)
                            {
                                throw new Exception(string.Format("Error checking schedules for break efficiciency for {0}", salesArea.Name), myTask.Exception);
                            }
                        });
                        tasks.Add(task);
                    }

                    // Wait for all tasks to complete
                    Task.WaitAll(tasks.ToArray());

                    if (countSalesAreasWithBreakEfficiencySet == 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Run {0} (Run ID {1}) did not generate any break efficiency data. There was a problem processing the output files", run.Description, run.Id), ""));
                    }

                    if (!results.Where(r => r.ResultType == SystemTestResult.ResultTypes.Error).Any())
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, "Break Efficiency output data OK", ""));
                    }
                }
            }
            catch (System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, string.Format("Error checking Break Efficiency output data: {0}", exception.Message), ""));
            }
            return results;
        }
    }
}
