using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using xggameplan.specification.tests.Extensions;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ScheduleRepositoryAdapter : RepositoryTestAdapter<Schedule, IScheduleRepository, int>
    {
        protected class CountBreaksAndProgrammesResult
        {
            public int BreakCount { get; set; }
            public int ProgrammeCount { get; set; }
        }

        public ScheduleRepositoryAdapter(IScenarioDbContext dbContext, IScheduleRepository repository) : base(dbContext, repository)
        {
        }

        protected override Schedule Add(Schedule model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Schedule> AddRange(params Schedule[] models)
        {
            throw new NotImplementedException();
        }

        protected override Schedule Update(Schedule model)
        {
            Repository.Update(model);
            return model;
        }

        protected override Schedule GetById(int id)
        {
            return Repository.Get(id);
        }

        protected override IEnumerable<Schedule> GetAll()
        {
            return Repository.GetAll();
        }

        protected override void Delete(int id)
        {
            Repository.Delete(id);
        }

        protected override void Truncate()
        {
            Repository.TruncateAsync().Wait();
        }

        protected override int Count()
        {
            return Repository.CountAll;
        }

        [RepositoryMethod]
        protected CallMethodResult GetSchedules(IDictionary<string, string> parameters)
        {
            return CallMethodResult.Create(
                Repository.GetSchedule(
                    parameters.GetBySpecflowService<List<string>>("salesAreaNames"),
                    parameters.GetBySpecflowService<DateTime>("fromDate"),
                    parameters.GetBySpecflowService<DateTime>("toDate")));
        }


        [RepositoryMethod]
        protected CallMethodResult CountBreaksAndProgrammes(DateTime dateFrom, DateTime dateTo)
        {
            var res = Repository.CountBreaksAndProgrammes(dateFrom, dateTo);

            return CallMethodResult.Create(new CountBreaksAndProgrammesResult
            {
                BreakCount = res.breaksCount,
                ProgrammeCount = res.programmesCount
            });
        }
    }
}
