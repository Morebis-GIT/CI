using System.Collections.Generic;
using xggameplan.CSVImporter;

namespace xggameplan.Repository
{
    public interface IFailureImportRepository
    {
        IEnumerable<FailureImportSummary> GetAll();
    }
}
