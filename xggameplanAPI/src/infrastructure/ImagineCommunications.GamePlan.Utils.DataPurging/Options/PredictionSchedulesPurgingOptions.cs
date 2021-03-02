namespace ImagineCommunications.GamePlan.Utils.DataPurging.Options
{
    public class PredictionSchedulesPurgingOptions : PurgingOptions
    {
        public ConcurrencyOptions Concurrency { get; set; } = new ConcurrencyOptions
        {
            ItemsPerTask = 2000,
            DegreeOfParallelism = 20
        };
    }
}
