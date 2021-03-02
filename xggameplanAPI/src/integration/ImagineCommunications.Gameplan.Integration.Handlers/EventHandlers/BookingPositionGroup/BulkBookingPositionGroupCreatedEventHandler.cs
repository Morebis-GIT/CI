using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.BusClient.Abstraction.Classes;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BookingPositionGroup;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.BookingPositionGroup
{
    public class BulkBookingPositionGroupCreatedEventHandler : EventHandler<IBulkBookingPositionGroupCreated>
    {
        private readonly IMapper _mapper;
        private readonly IBookingPositionGroupRepository _bookingPositionGroupRepository;

        public BulkBookingPositionGroupCreatedEventHandler(IMapper mapper, IBookingPositionGroupRepository bookingPositionGroupRepository)
        {
            _mapper = mapper;
            _bookingPositionGroupRepository = bookingPositionGroupRepository;
        }

        public override void Handle(IBulkBookingPositionGroupCreated command)
        {
            var bookingPositionGroups = _mapper.Map<List<GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects.BookingPositionGroup>>(command.Data);

            _bookingPositionGroupRepository.AddRange(bookingPositionGroups);
            _bookingPositionGroupRepository.SaveChanges();
        }
    }
}
