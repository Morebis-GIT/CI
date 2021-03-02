namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects
{
    /// <summary>
    /// Details for a Smooth default pass iteration
    /// </summary>
    public class SmoothPassDefaultIteration
    {
        public int Sequence { get; }

        public bool RespectCampaignClash { get; }

        public ProductClashRules ProductClashRules { get; }

        /// <summary>
        /// Whether to respect spot time requests. E.g. Spot only within first half of prog
        /// </summary>
        public bool RespectSpotTime { get; }

        /// <summary>
        /// Position of which break
        /// </summary>
        public SpotPositionRules BreakPositionRules { get; }

        /// <summary>
        /// Position of spot within break
        /// </summary>
        public SpotPositionRules RequestedPositionInBreakRules { get; }

        /// <summary>
        /// Whether to respect restrictions
        /// </summary>
        public bool RespectRestrictions { get; }

        /// <summary>
        /// Whether to respect clash exceptions
        /// </summary>
        public bool RespectClashExceptions { get; }

        public SmoothPassDefaultIteration(
            int sequence,
            bool respectSpotTime,
            bool respectCampaignClash,
            ProductClashRules productClashRules,
            SpotPositionRules breakPositionRules,
            SpotPositionRules requestedPositionInBreakRules,
            bool respectRestrictions,
            bool respectClashExceptions)
        {
            Sequence = sequence;
            RespectSpotTime = respectSpotTime;
            RespectCampaignClash = respectCampaignClash;
            ProductClashRules = productClashRules;
            BreakPositionRules = breakPositionRules;
            RequestedPositionInBreakRules = requestedPositionInBreakRules;
            RespectRestrictions = respectRestrictions;
            RespectClashExceptions = respectClashExceptions;
        }
    }
}
