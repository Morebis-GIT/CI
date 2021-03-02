using System.Collections.Generic;
using xggameplan.CSVImporter;

namespace xggameplan.Repository
{
    public interface IBaseRatingsImportRepository
    {
        IEnumerable<BaseRatingsImport> GetAll();
    }
}
