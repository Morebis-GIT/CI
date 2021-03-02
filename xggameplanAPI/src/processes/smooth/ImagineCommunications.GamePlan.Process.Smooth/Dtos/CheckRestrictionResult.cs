using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    /// <summary>
    /// A restriction check result.
    /// </summary>
    public readonly struct CheckRestrictionResult
    {
        public Restriction Restriction { get; }

        public RestrictionReasons Reason { get; }

        public CheckRestrictionResult(Restriction restriction, RestrictionReasons reason) =>
            (Restriction, Reason) = (restriction, reason);
    }
}
