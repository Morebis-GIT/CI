using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects
{
    /// <summary>
    /// Defines parameters for a default Smooth pass
    /// </summary>
    public class SmoothPassDefault : SmoothPass
    {
        /// <summary>
        /// Whether to include sponsored spots (null=Any)
        /// </summary>
        public bool? Sponsored { get; }

        /// <summary>
        /// Whether to include multipart spots (null=Any)
        /// </summary>
        public bool? HasMultipartSpots { get; }

        /// <summary>
        /// Whether to include preemptable spots (null=Any)
        /// </summary>
        public bool? Preemptable { get; }

        /// <summary>
        /// List of break requests to include (null item for no break request)
        /// </summary>
        public List<string> BreakRequests { get; }

        /// <summary>
        /// Whether to include spots with product clash code (null=Any)
        /// </summary>
        public bool? HasProductClashCode { get; }

        /// <summary>
        /// Whether multipart spots can be split over multiple breaks if necessary
        /// </summary>
        public bool CanSplitMultipartSpots { get; }

        public bool? HasSpotEndTime { get; }

        public SmoothPassDefault(
            int sequence,
            bool? sponsored,
            bool? hasMultipartSpots,
            bool canSplitMultipartSpots,
            bool? preemptable,
            List<string> breakRequests,
            bool? hasProductClashCode,
            bool? hasSpotEndTime)
        {
            Sequence = sequence;
            Sponsored = sponsored;
            HasMultipartSpots = hasMultipartSpots;
            CanSplitMultipartSpots = canSplitMultipartSpots;
            Preemptable = preemptable;
            BreakRequests = breakRequests;
            HasProductClashCode = hasProductClashCode;
            HasSpotEndTime = hasSpotEndTime;
        }
    }
}
