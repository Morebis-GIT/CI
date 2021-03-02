namespace ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.BreakAvailabilityCalculator
{
    public class RecalculateBreakAvailabilityOptions
    {
        /// <summary>
        /// Gets or sets the maximum number of calculate tasks that may be buffered for execution.
        /// </summary>
        public int BoundedCalculateTaskCapacity { get; set; }

        /// <summary>
        /// Gets or sets the number of breaks that may be saved into database by one bulk update operation.
        /// </summary>
        public int UpdateBreakBatchSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of bulk update operations that may be executed concurrently.
        /// </summary>
        public int UpdateBreakDegreeOfParallelism { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of DbContext operations that may be executed concurrently within calculate tasks.
        /// </summary>
        public int BoundedDbContextOperationCapacity { get; set; }
    }
}
