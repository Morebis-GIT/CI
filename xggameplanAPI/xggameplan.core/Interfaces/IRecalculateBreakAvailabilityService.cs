using System.Collections.Generic;
using System.Threading;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;

namespace xggameplan.core.Interfaces
{
    public interface IRecalculateBreakAvailabilityService
    {
        void Execute(
            DateTimeRange period,
            IEnumerable<SalesArea> salesAreas,
            CancellationToken cancellationToken = default);
    }
}
