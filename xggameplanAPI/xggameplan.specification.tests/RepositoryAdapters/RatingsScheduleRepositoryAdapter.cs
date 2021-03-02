using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class RatingsScheduleRepositoryAdapter : RepositoryTestAdapter<RatingsPredictionSchedule, IRatingsScheduleRepository, int>
    {
        public RatingsScheduleRepositoryAdapter(IScenarioDbContext dbContext, IRatingsScheduleRepository repository) : base(dbContext, repository)
        {
        }

        protected override RatingsPredictionSchedule Add(RatingsPredictionSchedule model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<RatingsPredictionSchedule> AddRange(params RatingsPredictionSchedule[] models)
        {
            Repository.Insert(models.ToList());
            return models;
        }

        protected override int Count()
        {
            return Repository.CountAll;
        }

        protected override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<RatingsPredictionSchedule> GetAll()
        {
            return Repository.GetAll();
        }

        protected override RatingsPredictionSchedule GetById(int id)
        {
            throw new NotImplementedException();
        }

        protected override void Truncate()
        {
            Repository.TruncateAsync().Wait();
        }

        protected override RatingsPredictionSchedule Update(RatingsPredictionSchedule model)
        {
            throw new NotImplementedException();
        }

        [RepositoryMethod]
        protected CallMethodResult RemoveRatingsSchedule(DateTime scheduleDay, string salesarea)
        {
            DbContext.WaitForIndexesAfterSaveChanges();
            var schedule = Repository.GetSchedule(scheduleDay, salesarea);
            Repository.Remove(schedule);
            DbContext.SaveChanges();
            return CallMethodResult.CreateHandled();
        }
    }
}
