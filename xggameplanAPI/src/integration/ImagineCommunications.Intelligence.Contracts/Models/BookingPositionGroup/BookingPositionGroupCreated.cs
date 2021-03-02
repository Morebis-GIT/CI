using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BookingPositionGroup;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.BookingPositionGroup
{
    public class BookingPositionGroupCreated : IBookingPositionGroupCreated
    {
        public BookingPositionGroupCreated(int groupId, string code, string description, int displayOrder, bool userDefined, IEnumerable<PositionGroupAssociation> positionGroupAssociations)
        {
            GroupId = groupId;
            Code = code;
            Description = description;
            DisplayOrder = displayOrder;
            UserDefined = userDefined;
            PositionGroupAssociations = positionGroupAssociations;
        }

        public int GroupId { get; }

        public string Code { get; }

        public string Description { get; }

        public int DisplayOrder { get; }

        public bool UserDefined { get; }

        public IEnumerable<PositionGroupAssociation> PositionGroupAssociations { get; }
    }
}
