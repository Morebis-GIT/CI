using xggameplan.CSVImporter;

// CMF Added

namespace xggameplan.Repository.CSV
{
    public class CSVSpotImportRepository : CSVImportRepositoryBase<SpotImport>, ISpotImportRepository
    {
        public CSVSpotImportRepository(CSVImportSettings importSettings)
        {
            _settings = importSettings;
        }
    }
}
