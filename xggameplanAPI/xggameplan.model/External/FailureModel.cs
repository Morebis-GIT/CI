namespace xggameplan.Model
{
    /// <summary>
    /// Scenario failure details
    /// </summary>
    public class FailureModel
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

        public string SalesAreaName { get; set; }

        public string SalesAreaShortName { get; set; }

        /// <summary>
        /// Number of failures
        /// </summary>
        public long Failures { get; set; }
    }


}
