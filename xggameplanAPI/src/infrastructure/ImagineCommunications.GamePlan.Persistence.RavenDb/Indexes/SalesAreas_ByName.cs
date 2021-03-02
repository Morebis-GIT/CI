using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class SalesAreas_ByName
        : AbstractIndexCreationTask<SalesArea>
    {
        public static string DefaultIndexName =>
            "SalesAreas/ByName";

        public SalesAreas_ByName()
        {
            Map = salesAreas =>
                from document in salesAreas
                select new
                {
                    document.Name
                };
        }
    }
}
