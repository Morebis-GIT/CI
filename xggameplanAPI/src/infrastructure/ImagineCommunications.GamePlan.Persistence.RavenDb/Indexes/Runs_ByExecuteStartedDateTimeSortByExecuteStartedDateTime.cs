using System.Linq;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Runs_ByExecuteStartedDateTimeSortByExecuteStartedDateTime
        : AbstractIndexCreationTask<Run>
    {
        public static string DefaultIndexName =>
            "Runs/ByExecuteStartedDateTimeSortByExecuteStartedDateTime";

        public Runs_ByExecuteStartedDateTimeSortByExecuteStartedDateTime()
        {
            Map = docs =>
                from doc in docs
                select new
                {
                    doc.ExecuteStartedDateTime
                };
        }
    }
}
