using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups
{
    public class PositionGroupAssociation : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int BookingPosition { get; set; }
        public int BookingOrder { get; set; }
        public int BookingPositionGroupId { get; set; }
    }
}
