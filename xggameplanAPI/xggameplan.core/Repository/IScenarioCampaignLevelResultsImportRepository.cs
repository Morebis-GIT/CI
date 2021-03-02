using System.Collections.Generic;
using xggameplan.CSVImporter;

namespace xggameplan.Repository
{
    public interface IScenarioCampaignLevelResultsImportRepository
    {
        IEnumerable<ScenarioCampaignLevelResultImport> GetAll();
    }
}
