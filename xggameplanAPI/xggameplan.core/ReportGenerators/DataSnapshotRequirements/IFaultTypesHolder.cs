using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;

namespace xggameplan.core.ReportGenerators.DataSnapshotRequirements
{
    public interface IFaultTypesHolder
    {
        IEnumerable<FaultType> FaultTypes { get; }
    }
}
