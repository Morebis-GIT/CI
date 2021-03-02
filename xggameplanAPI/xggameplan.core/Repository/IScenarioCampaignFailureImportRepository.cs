using System.Collections.Generic;
using xggameplan.CSVImporter;

namespace xggameplan.Repository
{
    public interface IScenarioCampaignFailureImportRepository
    {
        IEnumerable<ScenarioCampaignFailureImport> GetAll();
    }
}
