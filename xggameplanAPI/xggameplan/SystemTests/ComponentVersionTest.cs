using System;
using System.Collections.Generic;
using System.Linq;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.Model;
using xggameplan.Services;

namespace xggameplan.SystemTests
{
    /// <summary>
    /// Tests component versions
    /// </summary>
    internal class ComponentVersionTest : ISystemTest
    {
        private readonly IAutoBooks _autoBooks;
        private const string _category = "Component Version";

        public ComponentVersionTest(IAutoBooks autoBooks)
        {
            _autoBooks = autoBooks;
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
                // Get API version
                APIVersionModel apiVersion = GetVersion();
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, string.Format("API version: {0}", apiVersion.Version), ""));

                // Check binaries version
                if (String.IsNullOrEmpty(_autoBooks.Settings.BinariesVersion))
                {
                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "Binaries version: Not set. It will not be possible to start any runs", ""));
                }
                else
                {
                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, string.Format("Binaries version: {0}", _autoBooks.Settings.BinariesVersion), ""));
                }

                // Check AutoBook API version
                if (String.IsNullOrEmpty(_autoBooks.Settings.ApplicationVersion))
                {
                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, "AutoBook API (Default) version: Not set. It will not be possible to start any runs.", ""));
                }
                else
                {
                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, string.Format("AutoBook API (Default) version: {0}", _autoBooks.Settings.ApplicationVersion), ""));
                }

                // Check Provisioning API version
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Warning, _category, "Provisioning API version: Unknown (Not currently checked).", ""));

                // TODO: Add API to frontend
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Warning, _category, "Frontend version: Unknown (It is not currently possible to automatically determine the version).", ""));
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Warning, _category, "Database version: Unknown (It is not currently possible to automatically determine the version).", ""));

                results.AddRange(TestComponentCompatibility("", "", "", "", "", ""));
            }
            catch (System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Error, _category, string.Format("Error checking component versions: {0}", exception.Message), ""));
            }
            finally
            {
                if (!results.Where(r => r.ResultType == SystemTestResult.ResultTypes.Error).Any())
                {
                    results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Information, _category, "Version test OK.", ""));
                }
            }
            return results;
        }

        /// <summary>
        /// <para>Tests component compatibility.</para>
        /// <para>
        /// The long term plan is for each component to have a data structure defining its dependencies.
        /// </para>
        /// </summary>
        /// <param name="apiVersion"></param>
        /// <param name="frontendVersion"></param>
        /// <param name="databaseVersion"></param>
        /// <param name="binariesVersion"></param>
        /// <param name="autoBookApiVersion"></param>
        /// <param name="provisioningApiVersion"></param>
        /// <returns></returns>
        private List<SystemTestResult> TestComponentCompatibility(
            string apiVersion,
            string frontendVersion,
            string databaseVersion,
            string binariesVersion,
            string autoBookApiVersion,
            string provisioningApiVersion)
        {
            var results = new List<SystemTestResult>();
            results.Add(new SystemTestResult(
                SystemTestResult.ResultTypes.Warning,
                _category,
                "Version compatibility check is not currently implemented.", "")
                );

            return results;
        }

        private APIVersionModel GetVersion() =>
            VersionService.GetVersion();
    }
}
