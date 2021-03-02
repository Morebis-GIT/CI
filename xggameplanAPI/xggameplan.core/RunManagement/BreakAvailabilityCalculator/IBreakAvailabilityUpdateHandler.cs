using ImagineCommunications.GamePlan.Domain.Breaks;

namespace xggameplan.core.RunManagement.BreakAvailabilityCalculator
{
    public interface IBreakAvailabilityUpdateHandler<in TBreak>
        where TBreak : class, IBreakAvailability
    {
        void UpdateAvailability(TBreak theBreak);

        void UpdateOptimizerAvailability(TBreak theBreak);
    }
}
