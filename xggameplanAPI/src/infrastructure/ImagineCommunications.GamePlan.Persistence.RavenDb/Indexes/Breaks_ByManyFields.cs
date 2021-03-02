using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using Raven.Client.Indexes;
using Raven.Client.Linq.Indexing;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Breaks_ByManyFields
        : AbstractIndexCreationTask<Break>
    {
        public static string DefaultIndexName =>
            "Breaks/ByManyFields";

        public Breaks_ByManyFields()
        {
            Map = breaks =>
                from oneBreak in breaks
                select new
                {
                    oneBreak.Id,
                    SalesArea = oneBreak.SalesArea.Boost(10),
                    oneBreak.ScheduledDate,
                    oneBreak.ExternalBreakRef,
                    oneBreak.BroadcastDate,
                    oneBreak.SpotReferencesList
                };
        }
    }
}
