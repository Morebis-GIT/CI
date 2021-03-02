using xggameplan.CSVImporter;

namespace xggameplan.Repository.CSV
{
    public class CSVScenarioCampaignLevelResultsImportRepository : CSVImportRepositoryBase<ScenarioCampaignLevelResultImport>, IScenarioCampaignLevelResultsImportRepository
    {
        public CSVScenarioCampaignLevelResultsImportRepository(CSVImportSettings importSettings)
        {
            _settings = importSettings;
        }
    }
}
