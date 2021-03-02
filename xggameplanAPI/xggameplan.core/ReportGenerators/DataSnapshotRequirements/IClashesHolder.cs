using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;

namespace xggameplan.core.ReportGenerators.DataSnapshotRequirements
{
    public interface IClashesHolder
    {
        IEnumerable<Clash> Clashes { get; }
    }
}
