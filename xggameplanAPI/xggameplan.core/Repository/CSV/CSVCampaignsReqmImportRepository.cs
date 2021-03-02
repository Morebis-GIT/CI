using xggameplan.CSVImporter;

namespace xggameplan.Repository.CSV
{
    public class CSVCampaignsReqmImportRepository : CSVImportRepositoryBase<CampaignsReqmImport>, ICampaignsReqmImportRepository
    {
        public CSVCampaignsReqmImportRepository(CSVImportSettings importSettings)
        {
            _settings = importSettings;
        }
    }
}
