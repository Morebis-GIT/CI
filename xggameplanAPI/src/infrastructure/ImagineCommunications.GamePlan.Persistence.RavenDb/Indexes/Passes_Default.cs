using System.Linq;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    /// <summary>
    /// Define a RavenDB index for Pass documents.
    /// </summary>
    public class Passes_Default
        : AbstractIndexCreationTask<Pass>
    {
        /// <summary>
        /// The name of the default Passes documents index.
        /// </summary>
        public static string DefaultIndexName => "Passes/Default";

        public Passes_Default()
        {
            Map = passes => from pass in passes
                            select new
                            {
                                pass.Id,
                                pass.Name,
                                pass.DateCreated,
                                pass.DateModified
                            };

            Index(r => r.Name, FieldIndexing.Analyzed);
        }
    }
}
