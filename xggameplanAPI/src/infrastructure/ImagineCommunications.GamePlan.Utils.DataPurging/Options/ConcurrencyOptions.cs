namespace ImagineCommunications.GamePlan.Utils.DataPurging.Options
{
    public class ConcurrencyOptions

    {
        public int ItemsPerTask { get; set; } = 1;
        public int? DegreeOfParallelism { get; set; } = 1;
    }
}
