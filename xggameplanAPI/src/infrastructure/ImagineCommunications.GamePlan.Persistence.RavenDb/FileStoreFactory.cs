using System.Collections.Generic;
using Raven.Client.FileSystem;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb
{
    /// <summary>
    /// Files store factory.
    ///
    /// TODO: Move this to separate class library.
    /// </summary>
    public static class FileStoreFactory
    {
        public static IFilesStore CreateStore(string connectionString)
        {
            IFilesStore filesStore = new FilesStore()
            {
                Url = GetConnectionStringElements(connectionString)["Url"],
                DefaultFileSystem = GetConnectionStringElements(connectionString)["Database"]
            }.Initialize();

            ConfigureMaxRequest(filesStore);
            return filesStore;
        }

        /// <summary>
        /// Returns the elements of the connection string (E.g. URL, Database)
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetConnectionStringElements(string connectionString)
        {
            Dictionary<string, string> elements = new Dictionary<string, string>();
            string[] values = connectionString.Split(';');
            for (int index = 0; index < values.Length; index++)
            {
                string[] items = values[index].Split('=');

                elements.Add(items[0].Trim(), items[1].Trim());
            }
            return elements;
        }

        /// <summary>
        /// Set max number of requests
        /// </summary>
        /// <param name="documentStore"></param>
        private static void ConfigureMaxRequest(IFilesStore filesStore)
        {
            filesStore.Conventions.MaxNumberOfRequestsPerSession = 500000;
        }
    }
}
