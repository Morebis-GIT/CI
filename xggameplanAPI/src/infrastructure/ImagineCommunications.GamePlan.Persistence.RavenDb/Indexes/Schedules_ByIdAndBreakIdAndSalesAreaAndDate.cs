using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using Raven.Client.Indexes;
using Raven.Client.Linq.Indexing;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Schedules_ByIdAndBreakIdAndSalesAreaAndDate
        : AbstractIndexCreationTask<Schedule>
    {
        public static string DefaultIndexName => "Schedules/ByIdAndBreakIdAndSalesAreaAndDate";

        public Schedules_ByIdAndBreakIdAndSalesAreaAndDate()
        {
            Map = schedules =>
                from schedule in schedules
                select new
                {
                    schedule.Id,
                    Breaks_Id = schedule.Breaks.Select(b => b.Id),
                    SalesArea = schedule.SalesArea.Boost(10),
                    schedule.Date
                };
        }
    }
}
