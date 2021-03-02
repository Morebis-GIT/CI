using System.Linq;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class ISRSettings_BySalesArea
        : AbstractIndexCreationTask<ISRSettings>
    {
        public static string DefaultIndexName =>
            "ISRSettings/BySalesArea";

        public ISRSettings_BySalesArea()
        {
            Map = isrSettings =>
                from document in isrSettings
                select new
                {
                    document.SalesArea
                };
        }
    }
}
