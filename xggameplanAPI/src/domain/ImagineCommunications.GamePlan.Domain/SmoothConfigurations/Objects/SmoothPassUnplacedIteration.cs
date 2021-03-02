namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects
{
    /// <summary>
    /// Details for a Smooth unplaced pass iteration
    /// </summary>
    public class SmoothPassUnplacedIteration
    {
        public int Sequence { get; }

        public bool RespectSpotTime { get; }

        public bool RespectCampaignClash { get; }

        public ProductClashRules ProductClashRule { get; }

        public bool RespectRestrictions { get; }

        public bool RespectClashExceptions { get; }


        public SmoothPassUnplacedIteration(
            int sequence,
            bool respectSpotTime,
            bool respectCampaignClash,
            ProductClashRules productClashRule,
            bool respectRestrictions,
            bool respectClashExceptions)
        {
            Sequence = sequence;
            RespectSpotTime = respectSpotTime;
            RespectCampaignClash = respectCampaignClash;
            ProductClashRule = productClashRule;

            RespectRestrictions = respectRestrictions;
            RespectClashExceptions = respectClashExceptions;
        }
    }
}
