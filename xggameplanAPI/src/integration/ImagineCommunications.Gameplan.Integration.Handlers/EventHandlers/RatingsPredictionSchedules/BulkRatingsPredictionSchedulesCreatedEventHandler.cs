using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.BusClient.Abstraction.Classes;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.RatingsPredictionSchedules;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.RatingsPredictionSchedules
{
    public class BulkRatingsPredictionSchedulesCreatedEventHandler : EventHandler<IBulkRatingsPredictionSchedulesCreated>
    {
        private readonly IRatingsScheduleRepository _ratingsScheduleRepository;
        private readonly IMapper _mapper;

        public BulkRatingsPredictionSchedulesCreatedEventHandler(IRatingsScheduleRepository ratingsScheduleRepository, IMapper mapper)
        {
            _mapper = mapper;
            _ratingsScheduleRepository = ratingsScheduleRepository;
        }

        public override void Handle(IBulkRatingsPredictionSchedulesCreated command)
        {
            var ratingsPredictionSchedules = new List<RatingsPredictionSchedule>();

            foreach (var ratingsPredictionScheduleModel in command.Data)
            {
                var ratingsPredictionSchedule = _ratingsScheduleRepository.GetSchedule(ratingsPredictionScheduleModel.ScheduleDay.Date, ratingsPredictionScheduleModel.SalesArea);
                if (ratingsPredictionSchedule != null)
                {
                    _ratingsScheduleRepository.Remove(ratingsPredictionSchedule);
                }

                ratingsPredictionSchedules.Add(_mapper.Map<RatingsPredictionSchedule>(ratingsPredictionScheduleModel));
            }

            _ratingsScheduleRepository.Insert(ratingsPredictionSchedules, false);
            _ratingsScheduleRepository.SaveChanges();
        }
    }
}
