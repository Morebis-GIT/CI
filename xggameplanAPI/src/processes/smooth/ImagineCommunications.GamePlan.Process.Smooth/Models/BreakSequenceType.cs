namespace ImagineCommunications.GamePlan.Process.Smooth.Models
{
    public class BreakSequenceType
    {
        public int Order { get; internal set; }
        public BreakSequenceTypeCategory Category { get; internal set; }
        public string RequestedPositionInBreak { get; internal set; }
        public string MultipartSpotType { get; set; }

        /// <summary>
        /// Min sequence number
        /// </summary>
        public int MinSequence { get; internal set; }

        /// <summary>
        /// Max sequence number, typically used for Categories.MiddleOfBreak
        /// where there are multiple spots, each with a different sequence
        /// </summary>
        public int MaxSequence { get; internal set; }

        public BreakSequenceType(
            int order,
            BreakSequenceTypeCategory category,
            string requestPositionInBreak,
            string multipartSpotType,
            int minSequence)
        {
            Order = order;
            Category = category;
            RequestedPositionInBreak = requestPositionInBreak;
            MultipartSpotType = multipartSpotType;
            MinSequence = minSequence;
            MaxSequence = minSequence;
        }

        public BreakSequenceType(
            int order,
            BreakSequenceTypeCategory category,
            string requestPositionInBreak,
            string multipartSpotType,
            int minSequence,
            int maxSequence)
            : this(
                 order,
                 category,
                 requestPositionInBreak,
                 multipartSpotType,
                 minSequence
                 )
        {
            MaxSequence = maxSequence;
        }
    }
}
