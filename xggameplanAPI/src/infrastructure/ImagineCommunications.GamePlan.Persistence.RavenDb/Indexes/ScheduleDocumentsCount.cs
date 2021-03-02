using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class ScheduleDocumentsCount
        : AbstractIndexCreationTask<Schedule, ScheduleDocumentsCount.Result>
    {
        public class Result
        {
            public DateTime Date { get; set; }
            public int BreaksCount { get; set; }
            public int ProgrammesCount { get; set; }
        }

        public ScheduleDocumentsCount()
        {
            Map = schedules =>
                    from schedule in schedules
                    select new
                    {
                        Date = schedule.Date,
                        BreaksCount = schedule.Breaks.Count,
                        ProgrammesCount = schedule.Programmes.Count,
                    };

            Reduce = results => from result in results
                                group result by result.Date into g
                                select new
                                {
                                    Date = g.Key,
                                    BreaksCount = g.Sum(x => x.BreaksCount),
                                    ProgrammesCount = g.Sum(x => x.ProgrammesCount)
                                };
        }
    }
}
