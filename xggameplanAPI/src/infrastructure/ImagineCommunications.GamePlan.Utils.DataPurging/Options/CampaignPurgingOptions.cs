namespace ImagineCommunications.GamePlan.Utils.DataPurging.Options
{
    public class CampaignPurgingOptions : PurgingOptions
    {
        public ConcurrencyOptions Concurrency { get; set; } = new ConcurrencyOptions
        {
            ItemsPerTask = 200,
            DegreeOfParallelism = 1
        };
    }
}
