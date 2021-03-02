using NodaTime;

namespace ImagineCommunications.GamePlan.Domain.Breaks
{
    public static class Extensions
    {
        public static bool HasAvailabilityChanged(this IBreakAvailability breakAvailability, Duration duration) =>
            breakAvailability != null &&
            (breakAvailability.Avail != duration || breakAvailability.OptimizerAvail != duration);
    }
}
