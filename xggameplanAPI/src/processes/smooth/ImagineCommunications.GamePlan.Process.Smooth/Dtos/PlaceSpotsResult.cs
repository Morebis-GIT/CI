using System.Collections.Generic;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    /// <summary>
    /// Result of an attempt to place either a single spot, a group of related
    /// multipart spots or a small number of short duration spots (E.g. 3 x 10
    /// secs spots).
    /// </summary>
    public class PlaceSpotsResult
    {
        /// <summary>
        /// Break that spots should be placed in (null=No spots placed)
        /// </summary>
        public SmoothBreak SmoothBreak { get; set; }

        public string BestBreakFactorGroupName { get; set; }

        /// <summary>
        /// Spots that could be placed in the break SmoothBreak
        /// </summary>
        public List<PlaceSpotResult> PlacedSpotResults = new List<PlaceSpotResult>();

        /// <summary>
        /// Spots that could not be placed in any break
        /// </summary>
        public List<PlaceSpotResult> UnplacedSpotResults = new List<PlaceSpotResult>();
    }
}
