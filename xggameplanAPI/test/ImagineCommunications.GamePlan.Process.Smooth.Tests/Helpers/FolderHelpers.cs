using System;
using System.IO;
using System.Reflection;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers
{
    internal static class FolderHelpers
    {
        static FolderHelpers()
        {
#if NET5_0
            string executable = new Uri(Assembly.GetExecutingAssembly().Location).LocalPath;
#elif (NETCOREAPP3_1 || NET472)
            string executable = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
#endif

            TestDataRootDirectoryName =
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(executable)
                ));

            if (!Directory.Exists(TestDataRootDirectoryName))
            {
                throw new DirectoryNotFoundException(
                    $"Project folder [{TestDataRootDirectoryName}] was not found."
                    );
            }
        }

        /// <summary>
        /// Get the name of the project's folder. Add the subfolder names to
        /// this to reach the JSON test data.
        /// </summary>
        internal static string TestDataRootDirectoryName { get; }
    }
}
