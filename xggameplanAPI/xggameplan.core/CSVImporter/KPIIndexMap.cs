namespace xggameplan.CSVImporter
{
    public class KPIIndexMap : OutputFileMap<KPIImport>
    {
        public KPIIndexMap()
        {
            Map(m => m.CampaignCompletionLessThen5).Index(0);
            Map(m => m.CampaignCompletionFrom5To10).Index(1);
            Map(m => m.CampaignCompletionFrom10To15).Index(2);
            Map(m => m.CampaignCompletionFrom15To20).Index(3);
            Map(m => m.CampaignCompletionFrom20To25).Index(4);
            Map(m => m.CampaignCompletionFrom25To30).Index(5);
            Map(m => m.CampaignCompletionFrom30To35).Index(6);
            Map(m => m.CampaignCompletionFrom35To40).Index(7);
            Map(m => m.CampaignCompletionFrom40To45).Index(8);
            Map(m => m.CampaignCompletionFrom45To50).Index(9);
            Map(m => m.CampaignCompletionFrom50To55).Index(10);
            Map(m => m.CampaignCompletionFrom55To60).Index(11);
            Map(m => m.CampaignCompletionFrom60To65).Index(12);
            Map(m => m.CampaignCompletionFrom65To70).Index(13);
            Map(m => m.CampaignCompletionFrom70To75).Index(14);
            Map(m => m.CampaignCompletionFrom75To80).Index(15);
            Map(m => m.CampaignCompletionFrom80To85).Index(16);
            Map(m => m.CampaignCompletionFrom85To90).Index(17);
            Map(m => m.CampaignCompletionFrom90To95).Index(18);
            Map(m => m.CampaignCompletionFrom95To100).Index(19);
            Map(m => m.CampaignCompletionFrom100To105).Index(20);
            Map(m => m.CampaignCompletionFrom105To110).Index(21);
            Map(m => m.CampaignCompletionFrom110To115).Index(22);
            Map(m => m.CampaignCompletionOver115).Index(23);
            Map(m => m.AverageEfficiency).Index(24);
            Map(m => m.TotalSpotsBooked).Index(25);
            Map(m => m.RemainingAudience).Index(26);
            Map(m => m.RemainingAvailability).Index(27);
            Map(m => m.StandardAverageCompletion).Index(28);
            Map(m => m.WeightedAverageCompletion).Index(29);
        }
    }
}
