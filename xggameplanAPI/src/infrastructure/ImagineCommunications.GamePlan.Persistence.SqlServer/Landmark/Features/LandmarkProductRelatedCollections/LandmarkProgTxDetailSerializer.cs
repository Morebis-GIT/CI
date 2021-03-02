using System;
using NodaTime;
using xggameplan.AuditEvents;
using xggameplan.core.Services.OptimiserInputFilesSerialisers;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Landmark.Features.LandmarkProductRelatedCollections
{
    public class LandmarkProgTxDetailSerializer : ProgTxDetailSerializer
    {
        protected override DateTime AdjustScheduledEndTime(DateTime value) => value.AddSeconds(-1);

        public LandmarkProgTxDetailSerializer(IAuditEventRepository auditEventRepository, IClock clock)
            : base(auditEventRepository, clock)
        {
        }
    }
}
