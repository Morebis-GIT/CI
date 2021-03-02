using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts
{
    public class StandardDayPartSplit : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int DayPartId { get; set; }
        public double Split { get; set; }
    }
}
