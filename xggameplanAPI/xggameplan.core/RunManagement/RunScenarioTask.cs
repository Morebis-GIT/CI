using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.common.Services;
using xggameplan.RunManagement;
using static xggameplan.common.Helpers.LogAsString;
using AutoBookDomainObject = ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects.AutoBook;

namespace xggameplan.core.RunManagement
{
    public class RunScenarioTask
    {
        private readonly IAutoBooks _autoBooks;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly RunInstanceCreator _runInstanceCreator;

        public RunScenarioTask(
            IAutoBooks autoBooks,
            IRepositoryFactory repositoryFactory,
            IAuditEventRepository auditEventRepository,
            RunInstanceCreator runInstanceCreator)
        {
            _autoBooks = autoBooks;
            _repositoryFactory = repositoryFactory;
            _auditEventRepository = auditEventRepository;
            _runInstanceCreator = runInstanceCreator;
        }

        private void RaiseInfo(string message) =>
            _auditEventRepository.Insert(
                AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $"{LogEntryDiscriminator}{message}"
                    )
                );

        public void Execute(
            Run run,
            RunScenario scenario,
            IReadOnlyCollection<AutoBookInstanceConfiguration> autoBookInstanceConfigurationsForRun,
            double autoBookRequiredStorageGB,
            ConcurrentBag<RunInstance> runInstances,
            ConcurrentDictionary<Guid, ScenarioStatuses> newScenarioStatuses,
            ConcurrentDictionary<Guid, bool> scenarioSyncStatuses,
            bool autoDistributed)

        {
            AutoBookDomainObject autoBook = null;
            IAutoBook autoBookInterface = null;
            bool runStarted = false;
            RaiseInfo($"Begin Execute for  ScenarioID: { scenario.Id}");

            try
            {
                AutoBookInstanceConfiguration runAutoBookInstanceConfiguration = null;

                if (autoDistributed)
                {
                    RaiseInfo($"AutoDistributed - RunScenarioTask Execute Starting ScenarioID ={ scenario.Id}, RunID ={ run.Id}");

                    //create instance for scenario
                    RunInstance runInstance = _runInstanceCreator.Create(run.Id, scenario.Id);

                    RaiseInfo($"AutoDistributed - about to enter: {nameof(runInstance.UploadInputFilesAndCreateAutoBookRequest)}");
                    runInstance.UploadInputFilesAndCreateAutoBookRequest(autoBookInstanceConfigurationsForRun, autoBookRequiredStorageGB);
                    RaiseInfo($"AutoDistributed - returned from: {nameof(runInstance.UploadInputFilesAndCreateAutoBookRequest)}");

                    // Flag run as started
                    runStarted = true;
                    runInstances.Add(runInstance);
                    _ = newScenarioStatuses.TryRemove(scenario.Id, out _); // Don't update scenario status at the end

                    scenarioSyncStatuses[scenario.Id] = false;

                    RaiseInfo($"AutoDistributed - RunScenarioTask Execute Started ScenarioID ={ scenario.Id}, RunID ={ run.Id}");
                }
                else
                {
                    try
                    {
                        using (MachineLock.Create("xggameplan.AWSAutoBooks.GetFreeAutoBook",
                            new TimeSpan(0, 10, 0)))
                        {
                            foreach (var autoBookInstanceConfiguration in autoBookInstanceConfigurationsForRun)
                            {
                                autoBook = _autoBooks.GetFirstAdequateIdleAutoBook(autoBookInstanceConfiguration, autoBookRequiredStorageGB, true);

                                if (autoBook != null) // Got free AutoBook
                                {
                                    RaiseInfo($"Got Free AutoBook: {autoBook.Id} ConfigurationId: {autoBook.InstanceConfigurationId}");
                                    runAutoBookInstanceConfiguration = autoBookInstanceConfiguration;
                                    break;
                                }
                            }
                        }
                    }
                    catch (MachineLockTimeoutException)
                    {
                        RaiseInfo($"MachineLockTimeoutException in xggameplan.AWSAutoBooks.GetFreeAutoBook");
                    }
                    // Get autobook interface
                    autoBookInterface = (autoBook == null) ? null : _autoBooks.GetInterface(autoBook);

                    // Get free AutoBook instance, will be locked so that it can't be used elsewhere
                    if (autoBook != null) // Free AutoBook - start run
                    {
                        RaiseInfo($"Free Autobook - Starting ScenarioID ={ scenario.Id}, AutoBookID ={autoBook?.Id}, RunID ={ run.Id}, Instance Configuration = { runAutoBookInstanceConfiguration.Description }");

                        // Start run, exception will cause cleanup below
                        RunInstance runInstance = _runInstanceCreator.Create(run.Id, scenario.Id);

                        runInstance.UploadInputFilesStartAutoBookRun(autoBookInterface, autoBook);

                        // Flag run as started
                        runStarted = true;
                        runInstances.Add(runInstance);
                        _ = newScenarioStatuses.TryRemove(scenario.Id, out _); // Don't update scenario status at the end

                        scenarioSyncStatuses[scenario.Id] = false;
                        RaiseInfo($"Started ScenarioID ={ scenario.Id}, AutoBookID ={ autoBook?.Id}, RunID ={ run.Id}, Instance Configuration = { runAutoBookInstanceConfiguration?.Description }");
                    }
                    else // No free AutoBook, awaiting for provisioning
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                            $"No free AutoBook, awaiting for provisioning, waiting for existing AutoBooks to be Idle (RunID={run.Id}, ScenarioID={scenario.Id})"));
                        // Update scenario so that it can be retried later when an AutoBook becomes idle
                        RunManager.UpdateScenarioStatuses(_repositoryFactory, _auditEventRepository, run.Id, new List<Guid> { scenario.Id },
                            new List<ScenarioStatuses> { ScenarioStatuses.Scheduled }, new List<DateTime?> { null });

                        _ = newScenarioStatuses.TryRemove(scenario.Id, out _); // Don't update scenario status at the end
                        scenarioSyncStatuses[scenario.Id] = false;
                    }
                }
            }
            catch (System.Exception exception)
            {
                // Log exception but don't throw it. We want to try and start other scenarios
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, $"Error starting scenario (RunID={run.Id}, ScenarioID={scenario.Id}, AutoBookID={(autoBook == null ? "Unknown" : autoBook.Id)})", exception));
            }
            finally
            {
                // If we locked a free AutoBook instance but didn't start the scenario then reset to free, unlocks it.
                if (!runStarted && autoBook != null)
                {
                    autoBookInterface.ResetFree();
                }
            }
        }
    }
}
