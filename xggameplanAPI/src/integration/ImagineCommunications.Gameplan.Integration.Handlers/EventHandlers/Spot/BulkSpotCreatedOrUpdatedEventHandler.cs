using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Spot;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Spots;
using SpotDbObject = ImagineCommunications.GamePlan.Domain.Spots.Spot;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Spot
{
    public class BulkSpotCreatedOrUpdatedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkSpotCreatedOrUpdated>
    {
        private readonly IMapper _mapper;
        private readonly ISpotRepository _spotRepository;
        private readonly IBookingPositionRepository _bookingPositionRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;

        public BulkSpotCreatedOrUpdatedEventHandler(
            ISpotRepository spotRepository,
            IBookingPositionRepository bookingPositionRepository,
            ISalesAreaRepository salesAreaRepository,
            IMapper mapper)
        {
            _spotRepository = spotRepository;
            _bookingPositionRepository = bookingPositionRepository;
            _salesAreaRepository = salesAreaRepository;
            _mapper = mapper;
        }

        public override void Handle(IBulkSpotCreatedOrUpdated command)
        {
            ValidateBookingPositions(command);

            var salesAreaDictionary = _salesAreaRepository.GetAll().ToDictionary(x => x.Name, x => x.CustomId);

            var spotEntities = _mapper.Map<List<SpotDbObject>>(command.Data);
            spotEntities.ForEach(s =>
            {
                s.Uid = Guid.NewGuid();
                var salesAreaCustomId = salesAreaDictionary[s.SalesArea];
                s.ExternalBreakNo = s.ExternalBreakNo.GenerateBreakExternalRef(salesAreaCustomId, s.StartDateTime);
            });

            _spotRepository.InsertOrReplace(spotEntities);
            _spotRepository.SaveChanges();
        }

        private void ValidateBookingPositions(IBulkSpotCreatedOrUpdated spots)
        {
            var hasBookingPositionSpots = spots.Data.Where(s =>
                    s.BookingPosition != BookingPosition.NoDefaultPosition && s.BookingPosition > 0)
                .Select(c => c.BookingPosition).ToList().Distinct();
            var existedBookingPositions =
                _bookingPositionRepository.GetByPositions(hasBookingPositionSpots).ToList();
            var nonExistedBookingPositions = hasBookingPositionSpots.Except(existedBookingPositions.Select(c => c.Position));

            if (nonExistedBookingPositions.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.BookingPositionNotFound, "Booking positions doesn't exist" + string.Join(",", nonExistedBookingPositions.ToList()));
            }
        }
    }
}
