using System;
using TechTalk.SpecFlow;

namespace xggameplan.specification.tests.Infrastructure.EnvironmentSettings
{
    public class EnvironmentSettings : IEnvironmentSettings
    {
        private const string SpecflowTestsForSQLServerEnvVariableName = "specflowTestsForSQLServer";
        private const string SpecflowTestsForRavenDbEnvVariableName = "specflowTestsForRavenDb";

        private DataProvider? _defaultDataProvider;

        public DataProvider? GetDataProvider()
        {
            if (_defaultDataProvider != null)
            {
                return _defaultDataProvider.Value;
            }

            var sqlServerEnvVariableValue = GetEnvVariableValue(SpecflowTestsForSQLServerEnvVariableName);
            var ravenDbEnvVariableValue = GetEnvVariableValue(SpecflowTestsForRavenDbEnvVariableName);

            if (sqlServerEnvVariableValue && ravenDbEnvVariableValue)
            {
                throw new SpecFlowException($"Environment variables {SpecflowTestsForSQLServerEnvVariableName} and {SpecflowTestsForRavenDbEnvVariableName} cannot be both true.");
            }

            if (sqlServerEnvVariableValue && !ravenDbEnvVariableValue)
            {
                _defaultDataProvider = DataProvider.SqlServer;
            }

            if (ravenDbEnvVariableValue && !sqlServerEnvVariableValue)
            {
                _defaultDataProvider = DataProvider.RavenDb;
            }

            TestProgress.WriteLine(_defaultDataProvider == null
                ? "DataProvider is not defined"
                : $"Use {_defaultDataProvider} DataProvider");

            return _defaultDataProvider;
        }

        private bool GetEnvVariableValue(string variableName)
        {
            var defaultDataProviderEnvVariableValue =
                Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User);

            if (string.IsNullOrWhiteSpace(defaultDataProviderEnvVariableValue))
            {
                defaultDataProviderEnvVariableValue =
                    Environment.GetEnvironmentVariable(variableName,
                        EnvironmentVariableTarget.Process);
            }

            TestProgress.WriteLine(
                $"EnvironmentVariable {variableName}: {defaultDataProviderEnvVariableValue}");

            return bool.TryParse(defaultDataProviderEnvVariableValue, out bool result) && result;
        }
    }
}
