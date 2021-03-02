using xggameplan.CSVImporter;

namespace xggameplan.Repository.CSV
{
    public class CSVBaseRatingsImportRepository : CSVImportRepositoryBase<BaseRatingsImport>, IBaseRatingsImportRepository
    {
        public CSVBaseRatingsImportRepository(CSVImportSettings importSettings)
        {
            _settings = importSettings;
        }
    }
}
