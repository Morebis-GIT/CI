using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.TotalRatings;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.TotalRatings
{
    public class BulkTotalRatingCreatedEventHandler : IEventHandler<IBulkTotalRatingCreated>
    {
        private readonly IMapper _mapper;
        private readonly ITotalRatingRepository _totalRatingRepository;
        private readonly IDemographicRepository _demographicRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;

        public BulkTotalRatingCreatedEventHandler(
            ITotalRatingRepository totalRatingRepository,
            IDemographicRepository demographicRepository,
            ISalesAreaRepository salesAreaRepository,
            IMapper mapper)
        {
            _totalRatingRepository = totalRatingRepository;
            _demographicRepository = demographicRepository;
            _salesAreaRepository = salesAreaRepository;
            _mapper = mapper;
        }

        public void Handle(IBulkTotalRatingCreated command)
        {
            var totalRatings = _mapper.Map<List<TotalRating>>(command.Data);

            _demographicRepository.ValidateDemographics(totalRatings.Select(c => c.Demograph).ToList());
            _salesAreaRepository.ValidateSalesArea(totalRatings.Select(c => c.SalesArea).ToList());

            _totalRatingRepository.AddRange(totalRatings);
            _totalRatingRepository.SaveChanges();
        }
    }
}
