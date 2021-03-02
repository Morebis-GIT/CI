using System;
using System.IO;
using System.Reflection;

namespace ImagineCommunications.GamePlan.Core.Tests
{
    internal static class FolderHelpers
    {
        internal static string RootDataFolder =>
            Path.Combine(
                GetTestDataRootDirectoryName(),
                "Data"
                );

        internal static string TestRootDataFolder =>
            Path.Combine(
                RootDataFolder,
                "OutputFileProcessing"
                );

        /// <summary>
        /// Get the name of the project's folder. Add the subfolder names to
        /// this to reach the test data.
        /// </summary>
        internal static string GetTestDataRootDirectoryName()
        {
            string executable = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;

            string testDataDirectoryName =
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(executable)
                        )
                );

            if (!Directory.Exists(testDataDirectoryName))
            {
                throw new DirectoryNotFoundException($"Project folder [{testDataDirectoryName}] was not found.");
            }

            return testDataDirectoryName;
        }
    }
}
