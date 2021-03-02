using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups
{
    public class BookingPositionGroup : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool UserDefined { get; set; }

        public ICollection<PositionGroupAssociation> PositionGroupAssociations { get; set; } =
            new HashSet<PositionGroupAssociation>();
    }
}
