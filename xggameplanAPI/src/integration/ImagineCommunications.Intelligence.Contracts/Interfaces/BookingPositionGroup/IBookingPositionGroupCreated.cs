using System.Collections.Generic;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.BookingPositionGroup;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BookingPositionGroup
{
    public interface IBookingPositionGroupCreated : IEvent
    {
        int GroupId { get; }

        string Code { get; }

        string Description { get; }

        int DisplayOrder { get; }

        bool UserDefined { get; }

        IEnumerable<PositionGroupAssociation> PositionGroupAssociations { get; }
    }
}
