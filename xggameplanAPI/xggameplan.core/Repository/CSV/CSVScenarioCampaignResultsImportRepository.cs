using xggameplan.CSVImporter;

namespace xggameplan.Repository.CSV
{
    public class CSVScenarioCampaignResultsImportRepository : CSVImportRepositoryBase<ScenarioCampaignResultImport>, IScenarioCampaignResultsImportRepository
    {
        public CSVScenarioCampaignResultsImportRepository(CSVImportSettings importSettings)
        {
            _settings = importSettings;
        }
    }
}
