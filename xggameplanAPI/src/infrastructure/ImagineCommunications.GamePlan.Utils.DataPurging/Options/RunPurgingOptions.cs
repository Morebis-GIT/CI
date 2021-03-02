namespace ImagineCommunications.GamePlan.Utils.DataPurging.Options
{
    public class RunPurgingOptions : PurgingOptions
    {
        public int RunsAfter { get; set; }

        public ConcurrencyOptions Concurrency { get; set; } = new ConcurrencyOptions();
    }
}
