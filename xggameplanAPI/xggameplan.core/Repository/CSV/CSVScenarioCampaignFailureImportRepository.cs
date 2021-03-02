using xggameplan.CSVImporter;

namespace xggameplan.Repository.CSV
{
    public class CSVScenarioCampaignFailureImportRepository :
        CSVImportRepositoryBase<ScenarioCampaignFailureImport>,
        IScenarioCampaignFailureImportRepository
    {
        public CSVScenarioCampaignFailureImportRepository(CSVImportSettings importSettings)
        {
            _settings = importSettings;
        }
    }
}
