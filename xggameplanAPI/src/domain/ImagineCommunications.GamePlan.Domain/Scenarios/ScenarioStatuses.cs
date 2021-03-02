namespace ImagineCommunications.GamePlan.Domain.Scenarios
{
    /// <summary>
    /// Scenario statuses
    /// </summary>
    public enum ScenarioStatuses : short
    {
        /// <summary>
        /// Initial status
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Execute requested
        /// </summary>
        Scheduled = 1,

        /// <summary>
        /// Stops scenario being started by another RunManager
        /// </summary>
        Starting = 2,

        /// <summary>
        /// Smoothing
        /// </summary>
        Smoothing = 8,

        /// <summary>
        /// Run in progress, includes uploading of input files
        /// </summary>
        InProgress = 3,

        /// <summary>
        /// Downloading and processing results
        /// </summary>
        GettingResults = 4,

        /// <summary>
        /// Completed, results available
        /// </summary>
        CompletedSuccess = 5,

        /// <summary>
        /// Completed with error, no results available
        /// </summary>
        CompletedError = 6,

        /// <summary>
        /// Deleted (cancelled) manually
        /// </summary>
        Deleted = 7
    }
}
