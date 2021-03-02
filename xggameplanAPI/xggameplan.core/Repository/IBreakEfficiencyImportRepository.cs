using System.Collections.Generic;
using xggameplan.CSVImporter;

namespace xggameplan.Repository
{
    public interface IBreakEfficiencyImportRepository
    {
        IEnumerable<BreakEfficiencyImport> GetAll();
    }
}
