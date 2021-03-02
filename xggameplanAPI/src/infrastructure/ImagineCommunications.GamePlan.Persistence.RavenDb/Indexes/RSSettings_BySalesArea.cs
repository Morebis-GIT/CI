using System.Linq;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class RSSettings_BySalesArea
        : AbstractIndexCreationTask<RSSettings>
    {
        public static string DefaultIndexName =>
            "RSSettings/BySalesArea";

        public RSSettings_BySalesArea()
        {
            Map = rsSettings =>
                from document in rsSettings
                select new
                {
                    document.SalesArea
                };
        }
    }
}
