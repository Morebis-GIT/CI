using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class AutoBookInstanceConfiguration_ByIdAndDescription
        : AbstractIndexCreationTask<AutoBookInstanceConfiguration>
    {
        public static string DefaultIndexName => "AutoBookInstanceConfiguration/ByIdAndDescription";

        public AutoBookInstanceConfiguration_ByIdAndDescription()
        {
            Map = autobookInstanceConfigs =>
                from autobookConfig in autobookInstanceConfigs
                select new
                {
                    autobookConfig.Id,
                    autobookConfig.Description
                };
        }
    }
}
