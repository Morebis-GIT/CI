using System.Collections.Generic;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    /// <summary>
    /// Details for moving spots to free up room for another spot
    /// </summary>
    public class SpotMoveDetails
    {
        /// <summary>
        /// Spots to move
        /// </summary>
        public List<SmoothSpot> SmoothSpotsToMove = new List<SmoothSpot>();

        /// <summary>
        /// Break to move the corresponding spot in SmoothSpotsToMove to
        /// </summary>
        public List<SmoothBreak> SmoothBreaksToMoveTo = new List<SmoothBreak>();
    }
}
