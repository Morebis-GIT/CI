using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.Model;

namespace xggameplan.SystemTests
{
    /// <summary>
    /// AutoBook test
    /// </summary>
    internal class AutoBookTest : ISystemTest
    {
        private IAutoBooks _autoBooks;
        private IRepositoryFactory _repositoryFactory;
        private const string _category = "AutoBooks";

        public AutoBookTest(IAutoBooks autoBooks, IRepositoryFactory repositoryFactory)
        {
            _autoBooks = autoBooks;
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
                    // Get list of AutoBooks
                    var autoBookRepository = scope.CreateRepository<IAutoBookRepository>();
                    var autoBooks = autoBookRepository.GetAll();

                    // Check AutoBook provisioning settings
                    results.AddRange(ExecuteProvisioningTests(autoBooks.ToList()));

                    // Warn if no AutoBooks exist
                    if (autoBookRepository.GetAll().ToList().Count == 0)
                    {
                        if (!_autoBooks.Settings.AutoProvisioning)
                        {
                            results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "No AutoBook data exists and auto-provisioning is disabled. It will not be possible to execute any runs.", ""));
                        }
                    }
                    else
                    {
                        // Check connectivity to each AutoBook
                        int countWorkingAutoBooks = 0;
                        int countFreeAutoBooks = 0;

                        foreach (var autoBook in autoBooks)
                        {
                            try
                            {
                                IAutoBook autoBookInterface = _autoBooks.GetInterface(autoBook);
                                var autoBookStatus = autoBookInterface.GetStatus();
                                if (AutoBook.IsWorkingStatus(autoBookStatus))
                                {
                                    countWorkingAutoBooks++;
                                    if (autoBookStatus == AutoBookStatuses.Idle)
                                    {
                                        countFreeAutoBooks++;
                                    }
                                }
                                else if (autoBookStatus == AutoBookStatuses.Fatal_Error)
                                {
                                    if (!_autoBooks.Settings.AutoProvisioning)
                                    {
                                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Warning, _category, string.Format("AutoBook {0} has indicated that is has encountered a fatal error. It cannot be used for any runs while it is in this state. Please manually restart it.", autoBook.Id), ""));
                                    }
                                }
                            }
                            catch
                            {
                                if (autoBook.Status != AutoBookStatuses.Provisioning)    // Ignore error if Provisioning
                                {
                                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Warning, _category, string.Format("Error testing connection to AutoBook {0} ({1})", autoBook.Id, autoBook.Api), ""));
                                }
                            }
                        }

                        if (countWorkingAutoBooks == 0)
                        {
                            if (!_autoBooks.Settings.AutoProvisioning)
                            {
                                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "There are no working AutoBooks. It will not be possible to start any runs.", ""));
                            }
                        }
                        else if (countFreeAutoBooks == 0)
                        {
                            results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Warning, _category, string.Format("All {0} working AutoBooks are currently executing a run. It will not be possible to start any runs until existing runs have completed.", countWorkingAutoBooks), ""));
                        }
                        else if (results.Count == 0)
                        {
                            results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, string.Format("AutoBook test OK ({0} working AutoBooks)", countWorkingAutoBooks), ""));
                        }
                    }
                }
            }
            catch(System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Error checking AutoBooks: {0}", exception.Message), ""));
            }
            return results;
        }

        /// <summary>
        /// Executes provisioning tests
        /// </summary>
        /// <returns></returns>
        private List<SystemTestResult> ExecuteProvisioningTests(List<AutoBook> autoBooks)
        {
            List<SystemTestResult> results = new List<SystemTestResult>();

            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var autoBookInstanceConfigurationRepository = scope.CreateRepository<IAutoBookInstanceConfigurationRepository>();

                if (!_autoBooks.Settings.AutoProvisioning)   // Auto-provisioning enabled
                {
                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Warning, _category, "Automatic AutoBook provisioning is disabled. AutoBooks must be managed manually. " +
                            "Crashed AutoBooks will not be automatically restarted.", ""));
                }
                if (String.IsNullOrEmpty(_autoBooks.Settings.ApplicationVersion))
                {
                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "The AutoBook version is not set in the configuration. It is not possible to create AutoBook instances.", ""));
                }
                if (String.IsNullOrEmpty(_autoBooks.Settings.ProvisioningAPIURL))
                {
                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "The Provisioning API URL is not set in the configuration. It is not possible to create AutoBook instances.", ""));
                }

                if (_autoBooks.Settings.AutoProvisioning)
                {
                    var autoBookInstanceConfigurations = autoBookInstanceConfigurationRepository.GetAll();
                    if (!autoBookInstanceConfigurations.Any())
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "There are no AutoBook instance configurations. It is not possible to create AutoBook instances.", ""));
                    }
                    if (_autoBooks.Settings.MinInstances > 0)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Warning, _category, string.Format("The minimum number of AutoBook instances is set to {0} but, for cost reasons, it is recommended to be zero so that " +
                                                                                                "the instances are deleted when they are no longer needed.", _autoBooks.Settings.MinInstances), ""));
                    }
                }

                // Warn if AutoBooks are running the wrong versions
                if (!_autoBooks.Settings.Locked && !String.IsNullOrEmpty(_autoBooks.Settings.ApplicationVersion))     // Do nothing if provisioning in progress
                {
                    foreach (var autoBook in autoBooks)
                    {
                        IAutoBook autoBookInterface = _autoBooks.GetInterface(autoBook);
                        try
                        {
                            GetAutoBookVersionModel getAutoBookVersionModel = autoBookInterface.GetVersion();
                            if (getAutoBookVersionModel.Version.ToUpper() != _autoBooks.Settings.ApplicationVersion.ToUpper())
                            {
                                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Warning, _category, string.Format("AutoBook {0} is running version {1} but the settings indicate that it should be running {2}. Please check that auto-provisioning is working.", autoBook.Id, getAutoBookVersionModel.Version, _autoBooks.Settings.ApplicationVersion), ""));
                            }
                        }
                        catch { };      // Ignore error
                    }
                }

                if (!_autoBooks.Settings.Locked)    // Don't check if provisioning in progress
                {
                    // Warn if insufficient AutoBooks
                    if (_autoBooks.Settings.MinInstances > 0 && autoBooks.Count < _autoBooks.Settings.MinInstances)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Warning, _category, string.Format("The settings indicate a minimum of {0} AutoBooks but currently there are only {1} AutoBooks.", _autoBooks.Settings.MinInstances, autoBooks.Count), ""));
                    }

                    // Warn if too many AutoBooks
                    if (_autoBooks.Settings.MaxInstances > 0 && autoBooks.Count > _autoBooks.Settings.MaxInstances)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Warning, _category, string.Format("The settings indicate a maximum of {0} AutoBooks but currently there are {1} AutoBooks.", _autoBooks.Settings.MinInstances, autoBooks.Count), ""));
                    }
                }

                // Test AutoBooks API. If we can't contact it then we can't provision AutoBooks
                try
                {
                    bool testProvisioningResult = _autoBooks.TestProvisioning();
                }
                catch
                {
                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "There was an error testing the AutoBooks API. It will prevent AutoBooks from being provisioned, unprovisioned or restarted.", ""));
                }
            }
            return results;
        }
    }
}
