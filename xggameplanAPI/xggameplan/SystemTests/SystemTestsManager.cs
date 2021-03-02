using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using Microsoft.Extensions.Configuration;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.AWS;
using xggameplan.cloudaccess.Business;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.RunManagement;

namespace xggameplan.SystemTests
{
    /// <summary>
    /// Performs system tests, produces a list of results
    /// </summary>
    public class SystemTestsManager : ISystemTestsManager
    {
        private readonly IConfiguration _applicationConfiguration;
        private readonly IRunManager _runManager;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly AWSSettings _awsSettings;
        private readonly ICloudStorage _cloudStorage;
        private readonly IAutoBooks _autoBooks;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IIdentityGeneratorResolver _identityGeneratorResolver;

        public SystemTestsManager(IConfiguration configuration, IRunManager runManager, AWSSettings awsSettings, ICloudStorage cloudStorage,
                          IAuditEventRepository auditEventRepository, IAutoBooks autoBooks, IRepositoryFactory repositoryFactory,
                          IIdentityGeneratorResolver identityGeneratorResolver)
        {
            _applicationConfiguration = configuration;
            _runManager = runManager;
            _awsSettings = awsSettings;
            _auditEventRepository = auditEventRepository;
            _autoBooks = autoBooks;
            _cloudStorage = cloudStorage;
            _repositoryFactory = repositoryFactory;
            _identityGeneratorResolver = identityGeneratorResolver;
        }

        /// <summary>
        /// Returns list of system tests
        /// </summary>
        /// <returns></returns>
        private List<ISystemTest> GetSystemTests(SystemTestCategories systemTestCategory)
        {
            var systemTests = new List<ISystemTest>();
            systemTests.Add(new AuditEventTest(_auditEventRepository));
            systemTests.Add(new DatabaseConnectivityTest(_applicationConfiguration));
            systemTests.Add(new StaticDataTest(_repositoryFactory, _applicationConfiguration));
            systemTests.Add(new ScheduleDataTest(_repositoryFactory));
            systemTests.Add(new AutoBookTest(_autoBooks, _repositoryFactory));
            systemTests.Add(new ScenarioPassLibraryTest(_repositoryFactory));
            systemTests.Add(new RunExecuteTest(_runManager, _autoBooks, _repositoryFactory, RunExecuteTest.DefaultTemplateRunId, _auditEventRepository, 15, _identityGeneratorResolver));
            systemTests.Add(new RunHistoryTest(_repositoryFactory));
            systemTests.Add(new RunNotificationTest(_repositoryFactory));
            systemTests.Add(new EmailTest(_applicationConfiguration));
            systemTests.Add(new CloudTest(_cloudStorage, _awsSettings));
            systemTests.Add(new ComponentVersionTest(_autoBooks));
            return systemTests.Where(st => st.CanExecute(systemTestCategory)).ToList();
        }

        /// <summary>
        /// Executes tests
        /// </summary>
        /// <returns></returns>
        public List<SystemTestResult> ExecuteTests(SystemTestCategories systemTestCategory)
        {
            // Get all system tests to run for category
            var systemTests = GetSystemTests(systemTestCategory);

            // Run tests
            var allSystemTestResults = new List<SystemTestResult>();
            foreach (var systemTest in systemTests)
            {
                try
                {
                    allSystemTestResults.AddRange(systemTest.Execute(systemTestCategory));
                }
                catch { };      // Ignore
            }
            return allSystemTestResults;
        }
    }
}
