using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    /// <summary>
    /// Action to move spot(s) to another break
    /// </summary>
    internal class SmoothActionMoveSpotToBreak : SmoothAction
    {
        /// <summary>
        /// Spot(s) to move
        /// </summary>
        public IEnumerable<string> ExternalSpotRefs { get; set; }

        /// <summary>
        /// Break to move corresponding spot to
        /// </summary>
        public IEnumerable<string> ExternalBreakRefs { get; set; }

        public SmoothActionMoveSpotToBreak(
            int sequence,
            IEnumerable<string> externalSpotRefs,
            IEnumerable<string> externalBreakRefs)
        {
            Sequence = sequence;
            ExternalSpotRefs = externalSpotRefs;
            ExternalBreakRefs = externalBreakRefs;
        }
    }
}
