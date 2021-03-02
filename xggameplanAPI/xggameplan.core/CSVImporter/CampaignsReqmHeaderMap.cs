namespace xggameplan.CSVImporter
{
    /// <summary>
    /// Base Ratings Mapping
    /// </summary>
    public class CampaignsReqmHeaderMap : OutputFileMap<CampaignsReqmImport>
    {
        public CampaignsReqmHeaderMap()
        {
            //just the two fields we need from output file "LMKII_CAMP_REQM.OUT"
            //index starts at 0
            Map(m => m.requirement).Name("requirement");
            Map(m => m.total_supplied).Name("total_supplied");
        }
    }
}
