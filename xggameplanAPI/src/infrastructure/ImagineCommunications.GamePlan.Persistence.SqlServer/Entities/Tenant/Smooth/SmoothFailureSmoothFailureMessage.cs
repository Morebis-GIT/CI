
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth
{
    public class SmoothFailureSmoothFailureMessage: IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int SmoothFailureId { get; set; }
        public int SmoothFailureMessageId { get; set; }
    }
}
