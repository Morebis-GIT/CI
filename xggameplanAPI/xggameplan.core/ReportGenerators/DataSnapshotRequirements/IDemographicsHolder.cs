using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;

namespace xggameplan.core.ReportGenerators.DataSnapshotRequirements
{
    public interface IDemographicsHolder
    {
        IEnumerable<Demographic> Demographics { get; }
    }
}
