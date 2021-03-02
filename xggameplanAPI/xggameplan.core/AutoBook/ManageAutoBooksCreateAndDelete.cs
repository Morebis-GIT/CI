using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using Microsoft.Extensions.Configuration;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.Abstractions;

namespace xggameplan.AutoBooks
{
    public class ManageAutoBooksCreateAndDelete
    {
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IConfiguration _configuration;
        private readonly IAutoBookInstanceConfigurationRepository _autoBookInstanceConfigurationRepository;
        private readonly IAutoBooks _autoBooks;
        private readonly IRunRepository _runRepository;

        public ManageAutoBooksCreateAndDelete(
            IAuditEventRepository auditEventRepository,
            IConfiguration configuration,
            IAutoBookInstanceConfigurationRepository autoBookInstanceConfigurationRepository,
            IAutoBooks autoBooks,
            IRunRepository runRepository)
        {
            _auditEventRepository = auditEventRepository;
            _configuration = configuration;
            _autoBookInstanceConfigurationRepository = autoBookInstanceConfigurationRepository;
            _autoBooks = autoBooks;
            _runRepository = runRepository;
        }

        public void CreateAndDelete(Run run, AutoBookInstanceConfiguration autoBookInstanceConfiguration)
        {
            IEnumerable<AutoBook> allWorkingAutoBooks = _autoBooks.WorkingAutoBooks;
            var allAutoBookInstanceConfigurations = _autoBookInstanceConfigurationRepository.GetAll();
            bool singleInstance = allAutoBookInstanceConfigurations.Count == 1;

            RaiseInfo($"Enter ManageAutoBooksCreateAndDelete for runid: {run.Id}, AutoBooks.Instances: {_autoBooks.Instances.ToString()}");
            RaiseInfo($"Scenario count: {run.Scenarios.Count.ToString()}, max autobooks allowed: {_autoBooks.Settings.MaxInstances.ToString()}");
            RaiseInfo(singleInstance == true ?
                $"Single instance type available: {autoBookInstanceConfiguration.Id.ToString()}" :
                $"Varying instance types available, autobook type required: {autoBookInstanceConfiguration.Id.ToString()}");

            if (!singleInstance && allWorkingAutoBooks.Any())
            {
                var otherRunsAreActive = new Lazy<bool>(() => OtherActiveRunsWithWorkingScenarios(_runRepository, run.Id));
                var autobooksOfCorrectType = allWorkingAutoBooks.Where(w => w.InstanceConfigurationId >= autoBookInstanceConfiguration.Id).ToList();
                var autobooksOfTooSmallType = allWorkingAutoBooks.Where(w => w.InstanceConfigurationId < autoBookInstanceConfiguration.Id).ToList();
                RaiseInfo($"Varying Instance Types & Some working autobooks - Scenarios count: {run.Scenarios.Count} AutoBooks correct type: {autobooksOfCorrectType.Count}, AutoBooks too small type: {autobooksOfTooSmallType.Count}");

                if (run.Scenarios.Count > autobooksOfCorrectType.Count)
                {
                    foreach (RunScenario currentScenario in run.Scenarios)
                    {
                        if (run.Scenarios.Count > autobooksOfCorrectType.Count)
                        {
                            RaiseInfo($"Need autobooks: {run.Scenarios.Count}, Existing autobooks of required type: {autobooksOfCorrectType.Count}");

                            if (((autobooksOfCorrectType.Count + autobooksOfTooSmallType.Count) < _autoBooks.Settings.MaxInstances) || _autoBooks.Settings.MaxInstances == 0)
                            {
                                //create autobook
                                var newAutobook = CreateAutoBookOfType(autoBookInstanceConfiguration.Id);
                                autobooksOfCorrectType.Add(newAutobook);
                                RaiseInfo($"Within the max allowed so created one of suitable size: {newAutobook.Id.ToString()} autobooktype.Id: {autoBookInstanceConfiguration.Id.ToString()}");
                            }
                            else if (autobooksOfTooSmallType.Any())
                            {
                                if (!otherRunsAreActive.Value)
                                {
                                    RaiseInfo($"Max reached - checking for autobooks to delete, number of autobooks of too small type: {autobooksOfTooSmallType.Count.ToString()}, ScenarioID = {currentScenario.Id.ToString()}");
                                    var autoBook = GetAutoBookAbleToDelete(autobooksOfTooSmallType);
                                    if (autoBook != null)
                                    {
                                        //delete autobook
                                        var delid = autoBook.Id.ToString();
                                        DeleteAutoBook(autoBook);
                                        _ = autobooksOfTooSmallType.Remove(autoBook);

                                        //create autobook
                                        var newAutobook = CreateAutoBookOfType(autoBookInstanceConfiguration.Id);
                                        autobooksOfCorrectType.Add(newAutobook);
                                        RaiseInfo($"Max reached - deleted smaller autobook: {delid}, created larger autobook: {newAutobook.Id.ToString()}, autobooktype.Id: {autoBookInstanceConfiguration.Id.ToString()}, ScenarioID = {currentScenario.Id.ToString()}");
                                    }
                                    else
                                    {
                                        RaiseInfo("Max reached but couldn't delete an autobook (none with suitable status of Idle or Fatal Error)");
                                    }
                                }
                                else
                                {
                                    RaiseInfo("Other Active Runs, can't delete an autobook");
                                }
                            }
                        }
                    }
                }
            }

            else
            {
                RaiseInfo($"Single Instance Type or No working autobooks - scenario count: {run.Scenarios.Count.ToString()}, max allowed: {_autoBooks.Settings.MaxInstances.ToString()}, so creating autobooks of type: {autoBookInstanceConfiguration.Id.ToString()}");
                CreateAnAutoBookForEachScenario(run, autoBookInstanceConfiguration.Id, allWorkingAutoBooks.Count());
            }

            RaiseInfo($"Exit ManageAutoBooksCreateAndDelete for runid: {run.Id.ToString()}, AutoBooks.Instances: {_autoBooks.Instances.ToString()}");
        }

        public AutoBook CreateAutoBookOfType(int type)
        {
            var newAutoBook = new AutoBook()
            {
                TimeCreated = DateTime.UtcNow,
                Locked = false,
                Status = AutoBookStatuses.Provisioning,
                InstanceConfigurationId = type
            };
            _autoBooks.Create(newAutoBook);
            return newAutoBook;
        }

        public void DeleteAutoBook(AutoBook autoBook)
        {
            _autoBooks.Delete(autoBook);
            return;
        }

        private static bool OtherActiveRunsWithWorkingScenarios(IRunRepository runRepository, Guid currentRunId)
        {
            foreach (Run run in runRepository.GetAllActive())
            {
                if (run.Id == currentRunId)
                {
                    continue;
                }
                foreach (var scenario in run.Scenarios)
                {
                    if (scenario.Status == ScenarioStatuses.InProgress || scenario.Status == ScenarioStatuses.Scheduled || scenario.Status == ScenarioStatuses.Starting)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void CreateAnAutoBookForEachScenario(Run run, int autobookTypeId, int allWorkingAutoBooksCount)
        {
            int newlyCreatedAutoBooksCount = 0;

            foreach (RunScenario scenario in run.Scenarios)
            {
                if (
                    (run.Scenarios.Count > allWorkingAutoBooksCount + newlyCreatedAutoBooksCount)
                    && ((allWorkingAutoBooksCount + newlyCreatedAutoBooksCount < _autoBooks.Settings.MaxInstances) || _autoBooks.Settings.MaxInstances == 0))
                {
                    var newAutobook = CreateAutoBookOfType(autobookTypeId);
                    newlyCreatedAutoBooksCount++;
                }
            }
            RaiseInfo($"Created: {newlyCreatedAutoBooksCount} autobooks, autobooktype.Id: {autobookTypeId.ToString()}");
        }

        private AutoBook GetAutoBookAbleToDelete(List<AutoBook> autobooksOfTooSmallType)
        {
            foreach (AutoBook autobook in autobooksOfTooSmallType)
            {
                if (AutoBook.IsOKForDelete(autobook.Status))
                {
                    return autobook;
                }
            }
            return null;
        }

        private void RaiseInfo(string message) => _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"{message}"));
    }
}
