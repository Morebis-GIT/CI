using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class RatingsPredictionScheduleDomainModelHandler : IDomainModelHandler<RatingsPredictionSchedule>
    {
        private readonly IRatingsScheduleRepository _ratingsScheduleRepository;

        public RatingsPredictionScheduleDomainModelHandler(IRatingsScheduleRepository ratingsScheduleRepository)
        {
            _ratingsScheduleRepository = ratingsScheduleRepository ?? throw new ArgumentNullException(nameof(ratingsScheduleRepository));
        }

        public RatingsPredictionSchedule Add(RatingsPredictionSchedule model)
        {
            _ratingsScheduleRepository.Add(model);
            return model;
        }

        public void AddRange(params RatingsPredictionSchedule[] models) => _ratingsScheduleRepository.Insert(models.ToList());

        public int Count() => _ratingsScheduleRepository.CountAll;
        
        public void DeleteAll() => _ratingsScheduleRepository.TruncateAsync().Wait();

        public IEnumerable<RatingsPredictionSchedule> GetAll() => _ratingsScheduleRepository.GetAll();
    }
}
