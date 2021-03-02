using xggameplan.CSVImporter;

namespace xggameplan.Repository.CSV
{
    public class CSVBreakEfficiencyImportRepository : CSVImportRepositoryBase<BreakEfficiencyImport>, IBreakEfficiencyImportRepository
    {
        public CSVBreakEfficiencyImportRepository(CSVImportSettings importSettings)
        {
            _settings = importSettings;
        }
    }
}
