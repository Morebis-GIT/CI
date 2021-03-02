using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;

namespace xggameplan.core.ReportGenerators.DataSnapshotRequirements
{
    public interface ISalesAreasHolder
    {
        IEnumerable<SalesArea> SalesAreas { get; set; }
    }
}
