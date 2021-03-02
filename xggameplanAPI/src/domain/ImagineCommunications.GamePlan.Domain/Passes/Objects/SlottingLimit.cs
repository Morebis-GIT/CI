using System;

namespace ImagineCommunications.GamePlan.Domain.Passes.Objects
{
    /// <summary>
    /// Slotting Limit by demography
    /// </summary>
    public class SlottingLimit : ICloneable
    {
        /// <summary>
        /// Demography name
        /// </summary>
        public string Demographs { get; set; }

        /// <summary>
        /// Minimum Efficiency
        /// </summary>
        public int MinimumEfficiency { get; set; }

        /// <summary>
        /// Maximum Efficiency Rank
        /// </summary>
        public int MaximumEfficiency { get; set; }

        /// <summary>
        /// Demography Banding Tolerance
        /// </summary>
        public int BandingTolerance { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
