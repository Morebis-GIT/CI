namespace ImagineCommunications.GamePlan.Process.Smooth.Types
{
    public enum SponsorshipCompetitorType
    {
        /// <summary>
        /// The product is neither an advertiser or clash competitor to the sponsor.
        /// </summary>
        Neither,

        /// <summary>
        /// The product is an advertiser competitor to the sponsor.
        /// </summary>
        Advertiser,

        /// <summary>
        /// The product is a clash competitor to the sponsor.
        /// </summary>
        Clash,

        /// <summary>
        /// When a sponsorship restriction matches on both the product's advertiser
        /// and clash. We would then select the lower of the two restriction allowances.
        /// </summary>
        Both
    }
}
