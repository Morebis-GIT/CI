using System.Linq;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace xggameplan.RavenDB.Index
{
    public class Passes_IsLibrariedOnly
       : AbstractIndexCreationTask<Pass>
    {
        public static string DefaultIndexName => "Passes/IsLibrariedOnly";

        public Passes_IsLibrariedOnly()
        {
            Map = passes =>
                from pass in passes
                where pass.IsLibraried == true
                select new
                {
                    pass.Id
                };

            Index(r => r.Id, FieldIndexing.Analyzed);
        }
    }
}
