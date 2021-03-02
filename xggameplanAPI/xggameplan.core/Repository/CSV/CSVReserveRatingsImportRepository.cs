using xggameplan.CSVImporter;

namespace xggameplan.Repository.CSV
{
    public class CSVReserveRatingsImportRepository : CSVImportRepositoryBase<ReserveRatingsImport>, IReserveRatingsImportRepository
    {
        public CSVReserveRatingsImportRepository(CSVImportSettings importSettings)
        {
            _settings = importSettings;
        }
    }
}
