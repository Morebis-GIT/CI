namespace xggameplan.utils.seeddata.Seeding
{
    public interface IJsonFileImporter<T> : IJsonFileImporter
        where T : class
    {
    }

    /// <summary>
    /// Interface for importing a database
    /// </summary>
    public interface IJsonFileImporter
    {
        /// <summary>
        /// Import database
        /// </summary>
        bool Import(string dataFolder, bool replaceExistingData);

        /// <summary>
        /// Returns whether data exists of the specific type
        /// </summary>
        int GetDocumentCount();

        /// <summary>
        /// Removes existing data of the specific type
        /// </summary>
        void Delete();
    }
}
