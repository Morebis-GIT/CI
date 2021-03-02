using xggameplan.CSVImporter;

namespace xggameplan.Repository.CSV
{
    public class CSVConversionEfficiencyImportRepository : CSVImportRepositoryBase<ConversionEfficiencyImport>, IConversionEfficiencyImportRepository
    {
        public CSVConversionEfficiencyImportRepository(CSVImportSettings importSettings)
        {
            _settings = importSettings;
        }
    }
}
