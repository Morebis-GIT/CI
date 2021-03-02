namespace ImagineCommunications.GamePlan.Process.Smooth.Models
{
    /// <summary>
    /// Categories of break sequence type
    /// </summary>
    public enum BreakSequenceTypeCategory
    {
        /// <summary>
        /// Requested position in break
        /// </summary>
        RequestedPositionInBreak = 0,

        /// <summary>
        /// Multipart spot
        /// </summary>
        MultipartSpot = 1,

        /// <summary>
        /// Middle of break, spots that don't have a requested position
        /// in break
        /// </summary>
        MiddleOfBreak = 2
    }
}
