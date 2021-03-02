using System.Linq;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using Raven.Client.Indexes;
using Raven.Client.Linq.Indexing;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Passes_MinimalData : AbstractIndexCreationTask<Pass>
    {
        public static string DefaultIndexName => "Pass/Minimal";

        public Passes_MinimalData() => Map = passes =>
                                         from pass in passes
                                         select new
                                         {
                                             pass.Id,
                                             pass.Name,
                                             pass.DateModified,
                                             IsLibraried = pass.IsLibraried.Boost(10),
                                         };
    }
}