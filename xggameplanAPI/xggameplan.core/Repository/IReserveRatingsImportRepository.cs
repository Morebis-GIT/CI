using System.Collections.Generic;
using xggameplan.CSVImporter;

namespace xggameplan.Repository
{
    public interface IReserveRatingsImportRepository
    {
        IEnumerable<ReserveRatingsImport> GetAll();
    }
}
