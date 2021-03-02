using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// A null object for when restriction checking is disabled.
    /// </summary>
    /// <seealso cref="ImagineCommunications.GamePlan.Process.Smooth.Services.IRestrictionChecker" />
    public class NullRestrictionChecker
        : IRestrictionChecker
    {
        public List<CheckRestrictionResult> CheckRestrictions(
            Programme programme,
            Break oneBreak,
            Spot spot,
            SalesArea salesArea,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes)
        {
            return Enumerable.Empty<CheckRestrictionResult>().ToList();
        }

        public DebugRestrictionCheckerExitReason ExitReason =>
            DebugRestrictionCheckerExitReason.RestrictionCheckingIsDisabled;
    }
}
