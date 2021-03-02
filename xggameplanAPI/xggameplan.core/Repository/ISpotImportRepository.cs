using System.Collections.Generic;
using xggameplan.CSVImporter;

namespace xggameplan.Repository
{
    public interface ISpotImportRepository
    {
        IEnumerable<SpotImport> GetAll();
    }
}
