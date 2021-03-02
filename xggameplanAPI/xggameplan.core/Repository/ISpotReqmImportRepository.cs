using System.Collections.Generic;
using xggameplan.CSVImporter;

namespace xggameplan.Repository
{
    public interface ISpotReqmImportRepository
    {
        IEnumerable<SpotReqmImport> GetAll();
    }
}
