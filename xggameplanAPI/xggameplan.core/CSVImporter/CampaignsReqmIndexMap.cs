namespace xggameplan.CSVImporter
{
    /// <summary>
    /// Base Ratings Mapping
    /// </summary>
    public class CampaignsReqmIndexMap : OutputFileMap<CampaignsReqmImport>
    {
        public CampaignsReqmIndexMap()
        {
            //just the two fields we need from output file "LMKII_CAMP_REQM.OUT"
            //index starts at 0
            Map(m => m.requirement).Index(19);
            Map(m => m.total_supplied).Index(21);
        }
    }
}
