namespace xggameplan.CSVImporter
{
    public class ScenarioCampaignFailureHeaderMap : OutputFileMap<ScenarioCampaignFailureImport>
    {
        public ScenarioCampaignFailureHeaderMap()
        {
            Map(m => m.CampaignExternalId).Name("camp_external_ref");
            Map(m => m.SalesAreaNumber).Name("sare_no");
            Map(m => m.SpotLength).Name("sslg_length");
            Map(m => m.MultipartNumber).Name("multipart_no");
            Map(m => m.StrikeWeightStartDate).Name("stt_date");
            Map(m => m.StrikeWeightEndDate).Name("end_date");
            Map(m => m.DayPartStartTime).Name("stt_time");
            Map(m => m.DayPartEndTime).Name("end_time");
            Map(m => m.DayPartDays).Name("day_string");
            Map(m => m.FailureType).Name("type");
            Map(m => m.NumberOfFailures).Name("nbr_fails");
            Map(m => m.PassesEncounteringFailure).Name("passes");
        }
    }
}
