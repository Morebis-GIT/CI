using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class FunctionalAreas_Default : AbstractIndexCreationTask<FunctionalArea>
    {
        public static string DefaultIndexName => "FunctionalAreas/Default";

        public FunctionalAreas_Default()
        {
            Map = functionalAreas =>
                from functionalArea in functionalAreas
                select new
                {
                    functionalArea.Id,
                    functionalArea.Description,
                    functionalArea.FaultTypes
                };

            Index(r => r.Description, FieldIndexing.Analyzed);
        }
    }
}
