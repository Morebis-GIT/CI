using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Universes_Default
        : AbstractIndexCreationTask<Universe>
    {
        public static string DefaultIndexName => "Universes/Default";

        public Universes_Default()
        {
            string name = IndexName;
            Map = universes => from universe in universes
                               select new
                               {
                                   universe.SalesArea,
                                   universe.Demographic,
                                   universe.StartDate,
                                   universe.EndDate
                               };
        }
    }
}
