using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups
{
    public class BookingPosition : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public string Abbreviation { get; set; }
        public int BookingOrder { get; set; }
    }
}
