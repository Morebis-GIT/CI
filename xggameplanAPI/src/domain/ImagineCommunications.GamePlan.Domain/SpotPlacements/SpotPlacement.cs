using System;
using System.Collections.Generic;
using Raven.Client.UniqueConstraints;

namespace ImagineCommunications.GamePlan.Domain.SpotPlacements
{
    /// <summary>
    /// Details of the last placement for a spot.
    /// </summary>
    public class SpotPlacement
    {
        public int Id { get; set; }

        /// <summary>
        /// Time created. Used for deleting old instances for dead spots. E.g. Delete after 30 days if Spot doesn't exist.
        /// </summary>
        //public DateTime CreatedTime { get; set; }

        /// <summary>
        /// Time modified.
        /// </summary>
        public DateTime ModifiedTime { get; set; }

        /// <summary>
        /// Spot. We only care about the last placement for the spot, hence defined with unique constraint.
        /// </summary>
        [UniqueConstraint]
        public string ExternalSpotRef { get; set; }

        /// <summary>
        /// Break where spot was placed
        /// </summary>
        public string ExternalBreakRef { get; set; }

        /// <summary>
        /// Previous Break where spot was placed. Before updating ExternalBreakRef then we store the previous value in this property so that
        /// a reset of schedule data (including SpotPlacement) will reset ExternalBreakRef correctly.
        /// </summary>
        public string ResetExternalBreakRef { get; set; }

        public static Dictionary<string, SpotPlacement> IndexListByExternalSpotRef(IEnumerable<SpotPlacement> spotPlacements)
        {
            Dictionary<string, SpotPlacement> indexedSpots = new Dictionary<string, SpotPlacement>();
            foreach (var spotPlacement in spotPlacements)
            {
                if (!indexedSpots.ContainsKey(spotPlacement.ExternalSpotRef))
                {
                    indexedSpots.Add(spotPlacement.ExternalSpotRef, spotPlacement);
                }
            }
            return indexedSpots;
        }
    }
}
