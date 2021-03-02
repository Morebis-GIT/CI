using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.RunTypes
{
    /// <summary>
    /// List of hardcoded KPI names used in RunTypeAnalysisGroup records
    /// </summary>
    public static class RunTypeAnalysisGroupKPINames
    {
        public const string RatingsDelivery = "ratingsDelivery";
        public const string DeliveryPercentage = "deliveryPercentage";
        public const string RevenueBooked = "revenueBooked";
        public const string PoolValue = "poolValue";
        public const string Spots = "spots";
        public const string ZeroRatedSpots = "zeroRatedSpots";

        /// <summary>
        /// Retrieve all the KPI names in a single list
        /// </summary>
        public static HashSet<string> ListOfKPINames = new HashSet<string>
        {
            RatingsDelivery,
            DeliveryPercentage,
            RevenueBooked,
            PoolValue,
            Spots,
            ZeroRatedSpots
        };
    }
}
