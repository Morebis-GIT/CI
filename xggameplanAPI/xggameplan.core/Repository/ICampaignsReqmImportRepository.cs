using System.Collections.Generic;
using xggameplan.CSVImporter;

namespace xggameplan.Repository
{
    public interface ICampaignsReqmImportRepository
    {
        IEnumerable<CampaignsReqmImport> GetAll();
    }
}
