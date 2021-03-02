using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using Raven.Client.Indexes;

#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Clashes_ByUid
        : AbstractIndexCreationTask<Clash>
    {
        public static string DefaultIndexName => "Clashes/ByUid";

        public Clashes_ByUid()
        {
            Map = clashes =>
                from clash in clashes
                select new
                {
                    clash.Uid
                };
        }
    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores
