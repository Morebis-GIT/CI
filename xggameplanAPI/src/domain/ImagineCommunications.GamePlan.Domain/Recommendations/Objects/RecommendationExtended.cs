namespace ImagineCommunications.GamePlan.Domain.Recommendations.Objects
{
    public class RecommendationExtended
    {
        /// <summary>
        /// Sales Area Group Name
        /// </summary>
        public string SalesAreaGroupName { get; set; }

        /// <summary>
        /// Demographic Name
        /// </summary>
        public string DemographicName { get; set; }

        /// <summary>
        /// Client Name
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Parent Clash Name
        /// </summary>
        public string ParentClashName { get; set; }

        /// <summary>
        /// Parent Clash Exposure Level
        /// </summary>
        public int ParentClashExposureLevel { get; set; }

        /// <summary>
        /// Clash Name
        /// </summary>
        public string ClashName { get; set; }

        /// <summary>
        /// Clash Exposure Level
        /// </summary>
        public int ClashExposureLevel { get; set; }

        /// <summary>
        /// Product Name
        /// </summary>
        public string ProductName { get; set; }
    }
}
