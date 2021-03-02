namespace xggameplan.Database
{
    /// <summary>
    /// Interface for exporting a database
    /// </summary>
    public interface IDatabaseExporter
    {
        void Export(DatabaseMassProcessingSettings databaseExportSettings);
    }
}
