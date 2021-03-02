using System.Collections.Generic;
using xggameplan.CSVImporter;

namespace xggameplan.Repository
{
    public interface IConversionEfficiencyImportRepository
    {
        IEnumerable<ConversionEfficiencyImport> GetAll();
    }
}
