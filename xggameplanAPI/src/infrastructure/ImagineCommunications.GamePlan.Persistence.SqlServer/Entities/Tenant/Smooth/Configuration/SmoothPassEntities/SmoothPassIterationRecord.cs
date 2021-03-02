using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities
{
    public class SmoothPassIterationRecord: IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public int SmoothConfigurationId { get; set; }

        public ICollection<SmoothPassIterationRecordPassSequenceItem> PassSequences { get; set; }

        public SmoothPassIterationSpotsCriteria SpotsCriteria { get; set; }

        public SmoothPassDefaultIteration PassDefaultIteration { get; set; }

        public SmoothPassUnplacedIteration PassUnplacedIteration { get; set; }
    }

    public class SmoothPassIterationSpotsCriteria : SpotsCriteria
    {
    }
}
