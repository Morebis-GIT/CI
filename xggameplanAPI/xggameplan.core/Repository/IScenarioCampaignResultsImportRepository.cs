using System.Collections.Generic;
using xggameplan.CSVImporter;

namespace xggameplan.Repository
{
    public interface IScenarioCampaignResultsImportRepository
    {
        IEnumerable<ScenarioCampaignResultImport> GetAll();
    }
}
