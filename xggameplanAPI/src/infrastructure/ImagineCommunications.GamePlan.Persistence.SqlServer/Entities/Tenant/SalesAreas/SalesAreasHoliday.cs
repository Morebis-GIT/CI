using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas
{
    public class SalesAreasHoliday : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid SalesAreaId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public HolidayType Type { get; set; }
    }

    public enum HolidayType
    {
        Public,
        School
    }
}
