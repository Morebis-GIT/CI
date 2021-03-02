using System.Linq;
using ImagineCommunications.BusClient.Abstraction.Classes;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BookingPositionGroup;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.BookingPositionGroup
{
    public class BulkBookingPositionGroupDeletedEventHandler : EventHandler<IBulkBookingPositionGroupDeleted>
    {
        private readonly IBookingPositionGroupRepository _bookingPositionGroupRepository;

        public BulkBookingPositionGroupDeletedEventHandler(IBookingPositionGroupRepository bookingPositionGroupRepository) =>
            _bookingPositionGroupRepository = bookingPositionGroupRepository;

        public override void Handle(IBulkBookingPositionGroupDeleted command)
        {
            _bookingPositionGroupRepository.DeleteRangeByGroupId(command.Data.Select(c => c.Id));
            _bookingPositionGroupRepository.SaveChanges();
        }
    }
}
