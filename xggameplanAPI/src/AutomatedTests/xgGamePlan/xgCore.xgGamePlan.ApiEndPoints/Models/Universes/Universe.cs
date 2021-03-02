using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Universes
{
    /// <summary>
    /// Represents a Universe within GamePlan
    /// </summary>
    public class Universe
    {
        /// <summary>
        /// Represents a Universe within GamePlan
        /// </summary>
        public Guid Id { get; set; }
        public string SalesArea { get; set; }
        public string Demographic { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UniverseValue { get; set; }
    }
}
