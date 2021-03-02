using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    /// <summary>
    /// Action to move spot(s) to the unplaced spot list
    /// </summary>
    internal class SmoothActionMoveSpotToUnplaced : SmoothAction
    {
        /// <summary>
        /// Spot(s) to move
        /// </summary>
        public IEnumerable<string> ExternalSpotRefs { get; set; }

        public SmoothActionMoveSpotToUnplaced(
            int sequence,
            IEnumerable<string> externalSpotRefs)
        {
            Sequence = sequence;
            ExternalSpotRefs = externalSpotRefs;
        }
    }
}
