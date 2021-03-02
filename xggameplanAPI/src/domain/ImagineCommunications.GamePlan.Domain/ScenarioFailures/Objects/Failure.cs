namespace ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects
{
    /// <summary>
    /// Scenario failure details
    /// </summary>
    public class Failure
    {
        /// <summary>
        /// Campaign number
        /// </summary>
        public long Campaign { get; set; }

        /// <summary>
        /// Campaign Name
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        /// Campaign External Reference
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Failure type
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Number of failures
        /// </summary>
        public long Failures { get; set; }

        public string SalesAreaName { get; set; }

    }


}
