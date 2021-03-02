using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using Microsoft.Extensions.Configuration;
using Raven.Client;


namespace xggameplan.SystemTests
{
    /// <summary>
    /// Tests database connectivity
    /// </summary>
    internal class DatabaseConnectivityTest : ISystemTest
    {
        private IConfiguration _applicationConfiguration;
        private const string _category = "Database";

        public DatabaseConnectivityTest(IConfiguration applicationConfiguration)
        {
            _applicationConfiguration = applicationConfiguration;
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
                string connectionString = _applicationConfiguration["db:Master:connectionString"];
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(connectionString, System.Reflection.Assembly.GetExecutingAssembly()))
                {
                    var databaseConfiguration = documentStore.DatabaseCommands.Admin.GetDatabaseConfiguration();
                    if (databaseConfiguration != null)
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, "Database connectivity test OK", ""));
                    }
                    else
                    {
                        results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Database doesn't exist. Please check the connection string"), ""));
                    }
                }
            }
            catch (Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Unable to connect to database and run a query: {0}. Please check the connection to the Database Server", exception.Message), ""));
            }

            return results;
        }
    }
}
