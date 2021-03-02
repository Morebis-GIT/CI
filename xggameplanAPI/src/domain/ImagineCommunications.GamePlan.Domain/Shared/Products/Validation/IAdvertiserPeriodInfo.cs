using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;

namespace xggameplan.core.Validators.ProductAdvertiser
{
    /// <summary>
    /// Exposes advertiser identifier within the specified period
    /// </summary>
    public interface IAdvertiserPeriodInfo
    {
        /// <summary>Gets the period.</summary>
        /// <value>The period.</value>
        DateTimeRange Period { get; }

        /// <summary>Gets the external identifier.</summary>
        /// <value>The external identifier.</value>
        string ExternalIdentifier { get; }
    }
}
