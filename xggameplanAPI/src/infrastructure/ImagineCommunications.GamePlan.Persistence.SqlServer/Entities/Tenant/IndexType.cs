using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class IndexType : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int CustomId { get; set; }
        public string Description { get; set; }
        public Guid? SalesAreaId { get; set; }
        public string DemographicNo { get; set; }
        public string BaseDemographicNo { get; set; }
        public DateTime? BreakScheduleDate { get; set; }
    }
}
