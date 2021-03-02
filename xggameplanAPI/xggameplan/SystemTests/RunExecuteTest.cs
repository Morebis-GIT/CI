using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Generic.Extensions;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.Model;
using xggameplan.RunManagement;
using xggameplan.Services;

namespace xggameplan.SystemTests
{
    /// <summary>
    /// Tests run workflow. Executes a run, checks the results.
    /// </summary>
    internal class RunExecuteTest : ISystemTest
    {
        private readonly IRunManager _runManager;
        private readonly IAutoBooks _autoBooks;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly Guid _templateRunId;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly int _scenarioCompleteTimeoutMins;
        private readonly IIdentityGeneratorResolver _identityGeneratorResolver;
        private const string _category = "Run Execute";

        public static Guid DefaultTemplateRunId = new Guid("d3f6f448-f2b4-4438-b2b5-b95006781097");

        public RunExecuteTest(IRunManager runManager, IAutoBooks autoBooks, IRepositoryFactory repositoryFactory, Guid templateRunId,
            IAuditEventRepository auditEventRepository, int scenarioCompleteTimeoutMins,
            IIdentityGeneratorResolver identityGeneratorResolver)
        {
            _runManager = runManager;
            _autoBooks = autoBooks;
            _repositoryFactory = repositoryFactory;
            _templateRunId = templateRunId;
            _auditEventRepository = auditEventRepository;
            _scenarioCompleteTimeoutMins = scenarioCompleteTimeoutMins;
            _identityGeneratorResolver = identityGeneratorResolver;
        }

        public bool CanExecute(SystemTestCategories systemTestCategories)
        {
            return (systemTestCategories == SystemTestCategories.Deployment); // Only relevant for installation test
        }

        /// <summary>
        /// Creates run from template
        /// </summary>
        /// <param name="templateRun"></param>
        /// <param name="runRepository"></param>
        /// <returns></returns>
        private Run CreateRunFromTemplate(Run templateRun, IRunRepository runRepository)
        {
            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                // Get sales areas for the run
                var salesAreaRepository = scope.CreateRepository<ISalesAreaRepository>();

                var salesAreas = RunManager.GetSalesAreas(templateRun, salesAreaRepository.GetAll());

                // Set the default run start date time using today's date and time from template run, use it as the starting point to try and
                // find some schedule data.
                DateTime startingDateTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, templateRun.StartDate.Hour, templateRun.StartDate.Minute, templateRun.StartDate.Second, templateRun.StartDate.Millisecond, templateRun.StartDate.Kind);

                // Clone template run, reset it, set date range
                Run cloneRun = (Run)templateRun.Clone();
                cloneRun.Id = Guid.NewGuid();
                cloneRun.Description = $"Deployment Test ({GetVersion().Version})";
                cloneRun.CreatedDateTime = DateTime.UtcNow;
                cloneRun.ExecuteStartedDateTime = null;
                cloneRun.LastModifiedDateTime = cloneRun.CreatedDateTime;
                cloneRun.IsLocked = false;
                cloneRun.Scenarios.ForEach(scenario => scenario.ResetToPendingStatus());
                cloneRun.StartDate = GetRunStartDate(startingDateTime, salesAreas, 90);
                if (cloneRun.StartDate == DateTime.MaxValue)
                {
                    throw new Exception("Unable to determine start date for test run due to insufficient data");
                }
                cloneRun.EndDate = cloneRun.StartDate.AddDays(1);
                cloneRun.Real = false;     // Flag that it's not a real run

                // Clear IDs so that we can assign new ones
                //IdUpdater.ClearIds(cloneRun);
                cloneRun.Id = Guid.Empty;
                cloneRun.CustomId = 0;

                // Set new IDs
                IdUpdater.SetIds(cloneRun, _identityGeneratorResolver);

                // Save run
                runRepository.Add(cloneRun);
                runRepository.SaveChanges();
                return cloneRun;
            }
        }

        private APIVersionModel GetVersion() =>
            VersionService.GetVersion();

        /// <summary>
        /// Determines run start date, starts from starting date and tries to find a date with schedule data.
        /// </summary>
        /// <param name="startingDate"></param>
        /// <param name="salesAreas"></param>
        /// <param name="maxDaysToCheck"></param>
        /// <returns></returns>
        private DateTime GetRunStartDate(DateTime startingDate, List<SalesArea> salesAreas, int maxDaysToCheck)
        {
            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var scheduleRepository = scope.CreateRepository<IScheduleRepository>();
                for (int days = 0; days < maxDaysToCheck; days++)
                {
                    DateTime currentStartingDate = startingDate.AddDays(days);
                    var schedule = scheduleRepository.GetSchedule(salesAreas[0].Name, currentStartingDate.Date);
                    if (schedule != null && schedule.Breaks != null && schedule.Breaks.Count > 0)
                    {
                        return currentStartingDate;
                    }
                }
                return DateTime.MaxValue;
            }
        }

        public List<SystemTestResult> Execute(SystemTestCategories systemTestCategory)
        {
            var results = new List<SystemTestResult>();
            bool runCompleted = false;

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, "Executing test run"));

            try
            {
                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    var runRepository = scope.CreateRepository<IRunRepository>();

                    // Load template run
                    Run templateRun = runRepository.Find(_templateRunId);
                    if (templateRun == null)     // Template run does not exist, create it
                    {
                        CreateRunForTemplate(_templateRunId);
                        templateRun = runRepository.Find(_templateRunId);
                    }

                    if (templateRun != null)
                    {
                        // Create run from template
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, string.Format("Creating test run from template {0} ({1})", templateRun.Description, templateRun.Id)));
                        var run = CreateRunFromTemplate(templateRun, runRepository);
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, string.Format("Created test run (Run ID: {0})", run.Id)));

                        // Validate that run can be started
                        IEnumerable<SystemMessage> validationMessages = null;
                        try
                        {
                            validationMessages = _runManager.ValidateForStartRun(run);
                        }
                        catch (System.Exception exception)
                        {
                            results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Cannot perform test run because validation failed: {0}", exception.Message), ""));
                        }

                        if (!validationMessages.Any())
                        {
                            // Start run
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, string.Format("Starting test run {0} ({1})", run.Description, run.Id)));
                            var runInstances = StartRun(run, runRepository);
                            bool isRunStarted = runInstances.Any();

                            try
                            {
                                // If run not started (E.g. Provisioning, no free AutoBooks, no working AutoBooks) then try and do something to get them to start. Most likely then
                                // there's an AutoBook provisioning, in which case we just have to wait, but we may as well restart crashed instances too.
                                if (!isRunStarted)
                                {
                                    // Restart non-working AutoBooks
                                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, "Restarting any non-working AutoBooks"));
                                    int countRestarted = _autoBooks.RestartNonWorking().Count;
                                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, string.Format("Restarted {0} non-working AutoBooks", countRestarted)));

                                    // Wait for provisioning AutoBooks
                                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, "Waiting for any AutoBooks that are provisioning"));
                                    int countProvisioned = _autoBooks.WaitForProvisioned().Count;
                                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, string.Format("Waited for {0} AutoBooks that are provisioning", countProvisioned)));

                                    // Wait for an AutoBook to pick up the run
                                    if (countRestarted > 0 || countProvisioned > 0)
                                    {
                                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, "Waiting for any AutoBooks to start run"));
                                        DateTime wait = DateTime.UtcNow.AddSeconds(30);
                                        do
                                        {
                                            System.Threading.Thread.Sleep(200);
                                        } while (DateTime.UtcNow < wait);
                                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, "Waited for any AutoBooks to start run"));
                                    }

                                    // Refresh run details
                                    using (var runScope = scope.BeginRepositoryScope())
                                    {
                                        runRepository = runScope.CreateRepository<IRunRepository>();
                                        run = runRepository.Find(run.Id);
                                    }

                                    // Check if any scenario is running
                                    isRunStarted = run.Scenarios.Where(s => (s.IsScheduledOrRunning && s.Status != ScenarioStatuses.Scheduled) || (s.IsCompleted)).Any();
                                }

                                if (isRunStarted)        // Running
                                {
                                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, string.Format("Started test run (Scenarios started={0})", runInstances.Count)));

                                    // Wait for run to complete
                                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, "Waiting for run to complete"));
                                    runCompleted = WaitForRunCompleted(run.Id, TimeSpan.FromMinutes(_scenarioCompleteTimeoutMins));
                                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, string.Format("Waited for run to complete (Completed={0})", runCompleted)));

                                    // Refresh run details
                                    using (var runScope = scope.BeginRepositoryScope())
                                    {
                                        runRepository = runScope.CreateRepository<IRunRepository>();
                                        run = runRepository.Find(run.Id);
                                    }

                                    // Check run results
                                    ISystemTest runOutputTest = new RunOutputTest(run, _category, _repositoryFactory, _scenarioCompleteTimeoutMins);
                                    results.AddRange(runOutputTest.Execute(systemTestCategory));
                                }
                                else    // Run not started (E.g. Provisioning AutoBook, no working AutoBooks)
                                {
                                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "Unable to start test run", ""));
                                }
                            }
                            finally
                            {
                                // Refresh run details
                                using (var runScope = scope.BeginRepositoryScope())
                                {
                                    runRepository = runScope.CreateRepository<IRunRepository>();
                                    run = runRepository.Find(run.Id);
                                }

                                // Reset scenario status to not in progress otherwise other runs can't start
                                bool resetRun = false;
                                foreach (var scenario in run.Scenarios)
                                {
                                    if (Array.IndexOf(new ScenarioStatuses[] { ScenarioStatuses.Pending, ScenarioStatuses.CompletedError, ScenarioStatuses.CompletedSuccess, ScenarioStatuses.Deleted }, scenario.Status) == -1)
                                    {
                                        resetRun = true;
                                        scenario.ResetToPendingStatus();
                                    }
                                }
                                if (resetRun)
                                {
                                    runRepository.Add(run);
                                    runRepository.SaveChanges();
                                }
                            }
                        }
                        else     // Cannot execute run, validation message
                        {
                            foreach (var validationMessage in validationMessages)
                            {
                                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Cannot execute test run because validation failed: {0}", validationMessage.Description[Globals.SupportedLanguages[0]]), ""));
                            }
                        }
                    }
                }
            }
            catch (System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Error executing test run: {0}", exception.Message), ""));
            }

            if (results.Where(r => r.ResultType == SystemTestResult.ResultTypes.Error).Any())
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "Test run failed", ""));
            }
            else
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, "Test run OK", ""));
            }
            return results;
        }

        /// <summary>
        /// Creates template run, all run options selected, single sales area, default scenario. The template run can be manually modified
        /// later.
        /// </summary>
        /// <param name="runId"></param>
        private void CreateRunForTemplate(Guid runId)
        {
            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var repositories = scope.CreateRepositories(
                    typeof(IRunRepository),
                    typeof(ISalesAreaRepository),
                    typeof(ITenantSettingsRepository)
                );
                var runRepository = repositories.Get<IRunRepository>();
                var tenantSettingsRepository = repositories.Get<ITenantSettingsRepository>();
                var salesAreaRepository = repositories.Get<ISalesAreaRepository>();

                // Get sales areas
                var salesAreas = salesAreaRepository.GetAll();

                // Get tenant settings with default scenario
                var tenantSettings = tenantSettingsRepository.Get();

                // Create run, single sales area
                Run run = new Run()
                {
                    Id = runId,
                    Description = "Template",
                    CreatedDateTime = DateTime.UtcNow,
                    //StartDateTime = DateTime.UtcNow.Date,
                    //EndDateTime = DateTime.UtcNow.Date.AddDays(48),
                    IsLocked = false,
                    Optimisation = true,
                    Smooth = true,
                    ISR = true,
                    RightSizer = true,
                    Real = false,
                    Author = new AuthorModel()
                    {
                        Id = 1,
                        Name = "User"
                    }
                };
                run.SalesAreaPriorities.Add(new SalesAreaPriority()
                {
                    SalesArea = salesAreas.OrderBy(sa => sa.Name).First().Name,
                    Priority = SalesAreaPriorityType.Priority3
                });

                // Add scenario
                var runScenario = new RunScenario()
                {
                    Id = tenantSettings.DefaultScenarioId
                };
                run.Scenarios.Add(runScenario);
                //Scenario scenario = (Scenario)tenantSettings.DefaultScenario.Clone();
                run.Scenarios.ForEach(s => s.ResetToPendingStatus());
                //run.Scenarios.Add(scenario);

                // Set new IDs
                IdUpdater.SetIds(run, _identityGeneratorResolver);

                // Save run
                runRepository.Add(run);
                runRepository.SaveChanges();
            }
        }

        /// <summary>
        /// Starts the run
        /// </summary>
        /// <param name="run"></param>
        /// <param name="runRepository"></param>
        /// <returns></returns>
        private List<RunInstance> StartRun(Run run, IRunRepository runRepository)
        {
            DateTime executeStartedDateTime = DateTime.UtcNow;

            // Flag as scheduled
            run.ExecuteStartedDateTime = executeStartedDateTime;
            foreach (var scenario in run.Scenarios)
            {
                scenario.Status = ScenarioStatuses.Scheduled;
                scenario.StartedDateTime = executeStartedDateTime;  // Default, may be overriden for when scenario is actually started
            }
            runRepository.Update(run);
            runRepository.SaveChanges();   // Persist changes before we exit lock, don't wait for HTTP request handler to call SaveChanges
            return _runManager.AllScenariosStartRun(run.Id);
        }

        /// <summary>
        /// Waits for run to complete, all scenarios, or timeout
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private bool WaitForRunCompleted(Guid runId, TimeSpan timeout)
        {
            bool done = false;
            DateTime timeAbort = DateTime.UtcNow.Add(timeout);
            do
            {
                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    // Load run details
                    var runRepository = scope.CreateRepository<IRunRepository>();
                    var run = runRepository.Find(runId);

                    // Check completed scenarios
                    int scenariosCompleted = run.Scenarios.Where(s => s.IsCompleted).ToList().Count;
                    System.Diagnostics.Debug.WriteLine(string.Format("Waiting for run {0} to complete (Completed={1})", run.Id, scenariosCompleted));

                    if (scenariosCompleted == run.Scenarios.Count)  // All scenarios completed
                    {
                        done = true;
                    }
                    else if (DateTime.UtcNow < timeAbort)    // Wait before checking status again
                    {
                        System.Threading.Thread.Sleep(10000);
                    }
                }
            } while (done == false && DateTime.UtcNow < timeAbort);
            return done;
        }
    }
}
