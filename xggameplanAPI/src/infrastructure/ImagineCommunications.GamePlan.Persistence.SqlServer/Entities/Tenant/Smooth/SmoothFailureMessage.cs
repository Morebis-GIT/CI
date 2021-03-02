using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth
{
    public class SmoothFailureMessage
    {
        public int Id { get; set; }

        public ICollection<SmoothFailureMessageDescription> Descriptions { get; set; } =
            new List<SmoothFailureMessageDescription>(0);
    }
}
