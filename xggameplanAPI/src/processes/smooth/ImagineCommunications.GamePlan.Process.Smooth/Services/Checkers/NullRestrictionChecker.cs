using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// A null object for when restriction checking is disabled.
    /// </summary>
    internal class NullRestrictionChecker
        : IRestrictionChecker
    {
        private static readonly List<CheckRestrictionResult> _emptyList =
            Enumerable.Empty<CheckRestrictionResult>().ToList();

        public List<CheckRestrictionResult> CheckRestrictions(
            Programme programme,
            Break oneBreak,
            Spot spot,
            SalesArea salesArea,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes)
        {
            return _emptyList;
        }
    }
}
